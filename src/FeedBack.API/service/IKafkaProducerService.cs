using FeadBack.controller.dto;

namespace FeadBack.service;

public interface IKafkaProducerService
{
    Task SendFeedResponseAsync(KafkaFeedResponseDto dto, string topic = "feed-responses");
    Task SendFeedResponseAsync(int scope, string dealerName, string firstName, string lastName, string topic = "feed-responses");
    Task SendMultipleFeedResponsesAsync(IEnumerable<KafkaFeedResponseDto> dtos, string topic = "feed-responses");
    Task SendFeedResponsesWithDelayAsync(IEnumerable<KafkaFeedResponseDto> dtos, int delayMs = 100, string topic = "feed-responses");
}