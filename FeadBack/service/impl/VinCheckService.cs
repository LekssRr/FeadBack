namespace FeadBack.service.impl;
using FeadBack.controller.dto;
public class VinCheckService : IVinCheckService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<VinCheckService> _logger;

    public VinCheckService(HttpClient httpClient, ILogger<VinCheckService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<AutoResultDto> autoVinAsync(string vin)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/v1/auto/{vin}");
            
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning($"VIN проверка вернула {response.StatusCode} для VIN: {vin}");
                return null;
            }

            return await response.Content.ReadFromJsonAsync<AutoResultDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Ошибка при проверке VIN {vin}");
            throw new ApplicationException("Сервис проверки VIN временно недоступен");
        }
    }
}