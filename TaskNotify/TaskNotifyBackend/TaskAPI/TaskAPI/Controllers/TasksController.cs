using Confluent.Kafka;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace TaskAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TasksController> _logger;
        public TasksController(IConfiguration configuration, ILogger<TasksController> logger)
        {
            _logger = logger;
            _configuration = configuration;
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
                    _logger.LogInformation($"Delivery complete: {task.TaskId}");
                    return Ok(task);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Delivery failed: {ex}");
                    return StatusCode(StatusCodes.Status500InternalServerError,ex);
                }
            }
        }
    }
}
