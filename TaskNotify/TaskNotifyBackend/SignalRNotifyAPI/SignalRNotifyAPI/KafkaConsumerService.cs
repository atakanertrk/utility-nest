using Confluent.Kafka;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SignalRNotifyAPI.Hubs;
using System.Data;

namespace SignalRNotifyAPI
{
    public class KafkaConsumerService : BackgroundService
    {
        private readonly ILogger<KafkaConsumerService> _logger;
        private readonly ConsumerConfig _consumerConfig;
        private readonly IConfiguration _configuration;
        private readonly IHubContext<TaskNotifyHub> _hubContext;

        public KafkaConsumerService(ILogger<KafkaConsumerService> logger, IConfiguration configuration, IHubContext<TaskNotifyHub> hubContext)
        {
            _configuration = configuration;
            _logger = logger;
            _consumerConfig = new ConsumerConfig
            {
                GroupId = _configuration["KafkaSettings:GroupId"],
                BootstrapServers = _configuration["KafkaSettings:BootstrapServers"]
            };
            _hubContext = hubContext;
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
                consumer.Subscribe("tasks-processed");

                while (!stoppingToken.IsCancellationRequested)
                {
                    try
                    {
                        var consumeResult = consumer.Consume(stoppingToken);
                        var taskModel = JsonConvert.DeserializeObject<TaskModel>(consumeResult.Message.Value);
                        _logger.LogInformation($"Received Task: {taskModel.TaskName} by {taskModel.UserName}");

                        if (taskModel is not null)
                        {
                            NotifyUI(taskModel);
                        }
                    }
                    catch (OperationCanceledException ex)
                    {
                        _logger.LogError($"Tasks Processed Consume Exception: {ex}");
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Tasks Processed Consume Exception: {ex}");
                    }
                }

                consumer.Close();
            }
        }

        private async Task NotifyUI(TaskModel task)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveTaskNotification", task.UserName, task.TaskName);
        }
    }
}
