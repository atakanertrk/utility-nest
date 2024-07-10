using Confluent.Kafka;
using Confluent.SchemaRegistry.Serdes;
using Confluent.SchemaRegistry;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Confluent.Kafka;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Text;

namespace TaskAPI.Controllers
{
    //[Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public TasksController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> TestProduceTaskKafka([FromQuery] string taskName)
        {
            var config = new ProducerConfig { BootstrapServers = _configuration["KafkaSettings:BootstrapServers"], SecurityProtocol = SecurityProtocol.Plaintext, SaslMechanism = SaslMechanism.Plain };

            using (var p = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    var dr = await p.ProduceAsync("tasks-to-process", new Message<Null, string> { Value = JsonConvert.SerializeObject(new TaskModel() { TaskId = Guid.NewGuid(), TaskName = taskName, UserName = "PatricStar"}) });

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Delivery failed: {ex}");
                }
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> CreateNew([FromBody] TaskModel task)
        {
            var config = new ProducerConfig { BootstrapServers = _configuration["KafkaSettings:BootstrapServers"], SecurityProtocol = SecurityProtocol.Plaintext, SaslMechanism = SaslMechanism.Plain };

            using (var p = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    var dr = await p.ProduceAsync("tasks-to-process", new Message<Null, string> { Value = JsonConvert.SerializeObject(task) });
                    Console.WriteLine($"Delivery complete: {task.TaskId}");

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Delivery failed: {ex}");
                }
            }
            return Ok();
        }
    }
}
