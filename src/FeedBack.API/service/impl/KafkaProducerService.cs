using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FeadBack.controller.dto;

namespace FeadBack.service.impl;

public class KafkaProducerService : IKafkaProducerService, IDisposable
{
   private readonly IProducer<string, string> _producer;
    private readonly string _defaultTopic;
    private readonly JsonSerializerOptions _jsonOptions;

    public KafkaProducerService(
        string bootstrapServers = "localhost:9092",
        string defaultTopic = "feed-responses")
    {
        _defaultTopic = defaultTopic;

        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true,
            MessageSendMaxRetries = 3,
            LingerMs = 1,
            BatchSize = 16384,
            CompressionType = CompressionType.Snappy
        };

        _producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((_, error) => 
                Console.WriteLine($"Kafka Error: {error.Reason}"))
            .Build();

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task SendFeedResponseAsync(KafkaFeedResponseDto dto, string topic = "feed-responses")
    {
        try
        {
            var jsonMessage = JsonSerializer.Serialize(dto, _jsonOptions);
            var key = $"scope_{dto.Scope}_{Guid.NewGuid():N}";

            var message = new Message<string, string>
            {
                Key = key,
                Value = jsonMessage,
                Timestamp = new Timestamp(DateTime.UtcNow)
            };

            var deliveryResult = await _producer.ProduceAsync(topic, message);
            
            Console.WriteLine($"✅ Sent: {dto} → {deliveryResult.TopicPartitionOffset}");
        }
        catch (ProduceException<string, string> ex)
        {
            Console.WriteLine($"❌ Failed to send: {dto} - {ex.Error.Reason}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Unexpected error sending: {dto} - {ex.Message}");
        }
    }

    public async Task SendFeedResponseAsync(int scope, string dealerName, string firstName, string lastName, string topic = "feed-responses")
    {
        var dto = new KafkaFeedResponseDto
        {
            Scope = scope,
            DealerName = dealerName,
            FirstName = firstName,
            LastName = lastName
        };

        await SendFeedResponseAsync(dto, topic);
    }

    public async Task SendMultipleFeedResponsesAsync(IEnumerable<KafkaFeedResponseDto> dtos, string topic = "feed-responses")
    {
        var tasks = new List<Task>();
        var sentCount = 0;
        var errorCount = 0;

        foreach (var dto in dtos)
        {
            try
            {
                var task = SendFeedResponseAsync(dto, topic);
                tasks.Add(task);
                sentCount++;
            }
            catch (Exception ex)
            {
                errorCount++;
                Console.WriteLine($"❌ Error queuing: {dto} - {ex.Message}");
            }
        }

        await Task.WhenAll(tasks);
        Console.WriteLine($"📊 Sent: {sentCount}, Errors: {errorCount}");
    }

    public async Task SendFeedResponsesWithDelayAsync(IEnumerable<KafkaFeedResponseDto> dtos, int delayMs = 100, string topic = "feed-responses")
    {
        var sentCount = 0;
        var errorCount = 0;

        foreach (var dto in dtos)
        {
            try
            {
                await SendFeedResponseAsync(dto, topic);
                sentCount++;

                if (delayMs > 0)
                {
                    await Task.Delay(delayMs);
                }
            }
            catch (Exception ex)
            {
                errorCount++;
                Console.WriteLine($"❌ Error sending: {dto} - {ex.Message}");
                
                // Пауза при ошибке
                await Task.Delay(1000);
            }
        }

        Console.WriteLine($"📊 Sent with delay: {sentCount}, Errors: {errorCount}");
    }

    public void Flush()
    {
        _producer.Flush(TimeSpan.FromSeconds(10));
        Console.WriteLine("🔄 Producer flushed");
    }

    public void Dispose()
    {
        Flush();
        _producer?.Dispose();
        Console.WriteLine("♻️ Kafka producer disposed");
    }
}