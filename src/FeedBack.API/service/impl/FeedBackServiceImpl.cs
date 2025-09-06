using FeadBack.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using FeadBack;
using FeadBack.controller.dto;
using FeadBack.service;
public class FeedBackServiceImpl : FeedBackService
{
    
    private readonly AppDbContext _context;
    private readonly IVinCheckService _vinCheckService;
    private readonly ILogger<FeedBackServiceImpl> _logger;
    private readonly IClientCheckService _clientCheckService;
    private readonly IKafkaProducerService _kafkaProducerService;

    public FeedBackServiceImpl(AppDbContext context,
        IVinCheckService vinCheckService, 
        IClientCheckService clientCheckService,
        ILogger<FeedBackServiceImpl> logger,
        IKafkaProducerService kafkaProducerService) // Добавляем в конструктор
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _vinCheckService = vinCheckService ?? throw new ArgumentNullException(nameof(vinCheckService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _clientCheckService = clientCheckService ?? throw new ArgumentNullException(nameof(clientCheckService));
        _kafkaProducerService = kafkaProducerService ?? throw new ArgumentNullException(nameof(kafkaProducerService));
    }

    public FeedBackAutoDtoResponse createFeadBackAuto(FeedBackAutoDtoRequest feedBackAutoDtoRequest)
    {
        // Валидация
        if (string.IsNullOrWhiteSpace(feedBackAutoDtoRequest.vinAuto) || feedBackAutoDtoRequest.vinAuto.Length != 17)
            throw new ArgumentException("VIN должен содержать 17 символов");

        // Синхронная проверка VIN
        var autoInfo = _vinCheckService.autoVinAsync(feedBackAutoDtoRequest.vinAuto).GetAwaiter().GetResult();
        if (autoInfo == null)
            throw new ArgumentException("Автомобиль с указанным VIN не найден");

        // Синхронная проверка клиента
        var clientExists = _clientCheckService.CheckClient(new ClientResultDto(
            feedBackAutoDtoRequest.firstNameClient,
            feedBackAutoDtoRequest.lastNameClient,
            feedBackAutoDtoRequest.middleNameClient
        )).GetAwaiter().GetResult();

        if (!clientExists)
            throw new ArgumentException("Клиент не найден");
        
        // Создание записи
        var feadBack = new FeadBack.Models.FeadBack()
        {
            VinAuto = feedBackAutoDtoRequest.vinAuto,
            NameClient = $"{feedBackAutoDtoRequest.lastNameClient} {feedBackAutoDtoRequest.firstNameClient}",
            Description = feedBackAutoDtoRequest.description,
            DateTime = DateTime.UtcNow,
            feed = feedBackAutoDtoRequest.feed
        };

        // Синхронное сохранение
        _context.FeadBacks.Add(feadBack);
        _context.SaveChanges();
        // Отправка фидбэка в Kafka
        SendFeedbackToKafka(feedBackAutoDtoRequest, feadBack.Id);
        // Формирование ответа
        return new FeedBackAutoDtoResponse
        {
            model = autoInfo.BrandName,
            description = feedBackAutoDtoRequest.description,
            dataTime = DateTime.UtcNow,
            ServiceCompanies = autoInfo.ServiceCompanies ?? new List<ServiceCompanyDto>()
        };
    }
    private void SendFeedbackToKafka(FeedBackAutoDtoRequest request, int feedbackId)
    {
        try
        {
            // Создаем DTO для отправки в Kafka
            var kafkaMessage = new KafkaFeedResponseDto
            {
                Scope = request.feed, // Используем оценку как scope
                DealerName = request.dealerName, // Бренд + модель как дилер
                FirstName = request.firstNameEmploee, // Имя сотрудника
                LastName = request.lastNameEmploee    // Фамилия сотрудника
            };

            // Отправляем в Kafka
            _kafkaProducerService.SendFeedResponseAsync(kafkaMessage, "feedback-topic")
                .GetAwaiter()
                .GetResult(); // Синхронная отправка

            _logger.LogInformation("✅ Feedback sent to Kafka: {FeedbackId}, Score: {Score}", 
                feedbackId, request.feed);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error sending feedback to Kafka: {FeedbackId}", feedbackId);
            // Не бросаем исключение, чтобы не ломать основной flow
        }
    }
}