namespace FinstarApp.Kafka;

public interface IKafkaProducerService
{
    public Task ProduceAsync(string topic, string message);
}