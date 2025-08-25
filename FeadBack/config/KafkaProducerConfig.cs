namespace FeadBack.config;

using Confluent.Kafka;
using System;
using System.Threading.Tasks;

public class KafkaProducerConfig : KafkaSettings
{
    private readonly IProducer<string, string> _producer;
    private readonly string _defaultTopic;

    public KafkaProducerConfig(
        string bootstrapServers = "localhost:9092",
        string defaultTopic = "default-topic",
        Dictionary<string, string> customConfig = null)
    {
        _defaultTopic = defaultTopic;

        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true,
            MessageSendMaxRetries = 5,
            RetryBackoffMs = 300,
            LingerMs = 5,
            BatchSize = 16384,
            CompressionType = CompressionType.Lz4
        };

        // Добавляем кастомные настройки
        if (customConfig != null)
        {
            foreach (var item in customConfig)
            {
                config.Set(item.Key, item.Value);
            }
        }

        _producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler(HandleError)
            .SetLogHandler(HandleLog)
            .SetStatisticsHandler(HandleStatistics)
            .Build();
    }

    private void HandleError(IProducer<string, string> producer, Error error)
    {
        Console.WriteLine($"Kafka Error: {error.Code} - {error.Reason}");
    }

    private void HandleLog(IProducer<string, string> producer, LogMessage log)
    {
        Console.WriteLine($"Kafka Log: {log.Level} - {log.Message}");
    }

    private void HandleStatistics(IProducer<string, string> producer, string statistics)
    {
        Console.WriteLine($"Kafka Stats: {statistics}");
    }

    public async Task<DeliveryResult<string, string>> ProduceToTopicAsync(
        string topic, string message, string key = null, Headers headers = null)
    {
        var kafkaMessage = new Message<string, string>
        {
            Key = key,
            Value = message,
            Headers = headers,
            Timestamp = new Timestamp(DateTime.UtcNow)
        };

        return await _producer.ProduceAsync(topic, kafkaMessage);
    }

    public async Task ProduceWithHeadersAsync(
        string message, 
        string key = null, 
        Dictionary<string, string> headersDict = null)
    {
        var headers = new Headers();
        if (headersDict != null)
        {
            foreach (var header in headersDict)
            {
                headers.Add(header.Key, System.Text.Encoding.UTF8.GetBytes(header.Value));
            }
        }

        await ProduceToTopicAsync(_defaultTopic, message, key, headers);
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(15));
        _producer.Dispose();
    }
}