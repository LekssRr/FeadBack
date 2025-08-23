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

    public FeedBackServiceImpl(AppDbContext context,
        IVinCheckService vinCheckService, IClientCheckService clientCheckService,
        ILogger<FeedBackServiceImpl> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _vinCheckService = vinCheckService ?? throw new ArgumentNullException(nameof(vinCheckService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _clientCheckService = clientCheckService ?? throw new ArgumentNullException(nameof(clientCheckService));
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

        // Формирование ответа
        return new FeedBackAutoDtoResponse
        {
            model = autoInfo.BrandName,
            description = feedBackAutoDtoRequest.description,
            dataTime = DateTime.UtcNow,
            ServiceCompanies = autoInfo.ServiceCompanies ?? new List<ServiceCompanyDto>()
        };
    }
}