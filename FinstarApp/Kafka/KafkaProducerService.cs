using Confluent.Kafka;

namespace FinstarApp.Kafka;

public class KafkaProducerService : IKafkaProducerService
{
    private readonly ILogger<KafkaProducerService> _logger;
    private readonly IProducer<Null, string> _producer;
    private readonly string?[] _topics;
    
    public KafkaProducerService(IConfiguration configuration, ILogger<KafkaProducerService> logger)
    {
        _logger = logger;
        
        _topics = configuration.GetSection("Kafka:Topics")
                               .GetChildren()
                               .Select(section => section.ToString())
                               .ToArray();

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"]
        };
        _producer = new ProducerBuilder<Null, string>(producerConfig).Build();
    }

    public async Task ProduceAsync(string topic, string message)
    {
        if (_topics.Contains(topic) is false)
        {
            _logger.LogError("Kafka topic '{topic}' is not configured.", topic);
            return;
        }

        try
        {
            await _producer.ProduceAsync(
                topic,
                new Message<Null, string>
                {
                    Value = message
                }
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
        }
    }
}