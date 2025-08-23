namespace FeadBack.service.impl;
using Confluent.Kafka;


public class KafkaProducerService : IKafkaProducerService, IDisposable
{
    private readonly IProducer<Null, string> _producer;
    private readonly string _topic;
    
    public void Dispose()
    {
        throw new NotImplementedException();
    }
}