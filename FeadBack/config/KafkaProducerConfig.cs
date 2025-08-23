namespace FeadBack.config;

using Confluent.Kafka;
using System;
using System.Threading.Tasks;

public class KafkaProducerConfig : KafkaSettings
{
    public string Topic { get; set; }
    public bool EnableIdempotence { get; set; } = true;
    public CompressionType CompressionType { get; set; } = CompressionType.Snappy;
}