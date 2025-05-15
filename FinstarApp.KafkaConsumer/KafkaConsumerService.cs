using System.Text.Json;
using Confluent.Kafka;

namespace FinstarApp.KafkaConsumer;

public class KafkaConsumerService : BackgroundService
{
    private const int Requests = 3;
    
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly SemaphoreSlim _semaphore = new(Requests);

    public KafkaConsumerService(IConfiguration config, ILogger<KafkaConsumerService> logger)
    {
        _logger = logger;
        
        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = config["Kafka:BootstrapServers"],
            GroupId = "tasks-consumer-group",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        
        _consumer.Subscribe(config["Kafka:Topics:Tasks"]);
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka consumer service is starting.");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumed = _consumer.Consume(stoppingToken);
                
                await _semaphore.WaitAsync(stoppingToken);
                var info = JsonSerializer.Deserialize<TaskInfo>(consumed.Message.Value);
                
                _logger.LogInformation(
                    $"TaskId: {info.TaskId}; OldStatus: {info.OldStatus}; NewStatus: {info.NewStatus}; " +
                    $"UpdatedAt: {info.UpdatedAt}"
                );
                
                _semaphore.Release();
                _consumer.Commit(consumed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                await Task.Delay(5000, stoppingToken);
            }
        }
        
        _logger.LogInformation("Stopping Kafka consumer service.");
        _consumer.Close();
    }
}