using Confluent.Kafka;
using Dapper;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;

namespace CoreAPI
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly ConsumerConfig _consumerConfig;
        private readonly IConfiguration _configuration;

        public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _logger = logger;
            _consumerConfig = new ConsumerConfig
            {
                GroupId = _configuration["KafkaSettings:GroupId"],
                BootstrapServers = _configuration["KafkaSettings:BootstrapServers"]
            };
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                execute(stoppingToken);
            });
        }

        private void execute(CancellationToken stoppingToken)
        {
            using (var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build())
            {
                consumer.Subscribe("tasks-to-process");

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(stoppingToken);
                        var taskModel = JsonConvert.DeserializeObject<TaskModel>(consumeResult.Message.Value);
                        _logger.LogInformation($"Received Task To Process: {taskModel.TaskName} by {taskModel.UserName}");

                        if (taskModel is not null)
                        {
                            ProcessTask(taskModel, taskModel.TaskProcessTimeInSeconds); // process task with specified seconds delay in background
                        }
                    }
                    catch (ConsumeException ex)
                    {
                        _logger.LogError($"Consume Task To Process Exception: {ex}");
                    }
                    catch (OperationCanceledException ex)
                    {
                        _logger.LogError($"Consume Task To Process Exception: {ex}");
                        break;
                    }
                }

                consumer.Close();
            }
        }

        /// <summary>
        /// Mocks the process with delay
        /// </summary>
        /// <returns></returns>
        private async Task ProcessTask(TaskModel taskModel, int delayInSeconds)
        {
            await Task.Delay(delayInSeconds*1000);

            bool processResultDb = await ProcessTaskIntoDB(taskModel);
            if (processResultDb)
            {
                if (await ProduceTaskCompleted(taskModel))
                {
                    _logger.LogInformation($"Task Process Completed: {taskModel.TaskName} by {taskModel.UserName}");
                }
            }
        }

        private async Task<bool> ProcessTaskIntoDB(TaskModel task)
        {
            var connectionString = _configuration["MSSQLDb:ConnectionString"];

            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("p_task_id", task.TaskId, DbType.Guid, ParameterDirection.Input);
                    parameters.Add("p_task_name", task.TaskName, DbType.String, ParameterDirection.Input);
                    parameters.Add("p_user_name", task.UserName, DbType.String, ParameterDirection.Input);
                    parameters.Add("p_record_date", DateTime.Now, DbType.DateTime, ParameterDirection.Input);
                    await connection.ExecuteAsync("insert_user_task", parameters,commandType: CommandType.StoredProcedure);

                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"ProcessTaskIntoDB Failed {task.UserName}:{task.TaskName} {ex}");
                }
            }

            return false;
        }

        private async Task<bool> ProduceTaskCompleted(TaskModel task)
        {
            var config = new ProducerConfig { BootstrapServers = _configuration["KafkaSettings:BootstrapServers"], SecurityProtocol = SecurityProtocol.Plaintext, SaslMechanism = SaslMechanism.Plain };

            using (var p = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    var deliveryResult = await p.ProduceAsync("tasks-processed", new Message<Null, string> { Value = JsonConvert.SerializeObject(task) });
                    return deliveryResult.Status == PersistenceStatus.Persisted;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Task Processed Delivery failed: {task.UserName}:{task.TaskName} {ex}");
                }
            }
            return false;
        }
    }
}
