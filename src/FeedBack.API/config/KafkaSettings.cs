namespace FeadBack.config;
using Confluent.Kafka;
public class KafkaSettings
{
    public string BootstrapServers { get; set; }
    public string ClientId { get; set; } = "default-client";
    public int MessageTimeoutMs { get; set; } = 5000;
    public Acks Acks { get; set; } = Acks.All;
}