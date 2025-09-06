using System.Net.Http.Json;
using FeadBack.controller.dto;
using Microsoft.Extensions.Logging;
using System;
using System.Web;

namespace FeadBack.service.impl;

public class ClientCheckService : IClientCheckService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ClientCheckService> _logger;
    
    public ClientCheckService(HttpClient httpClient, ILogger<ClientCheckService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> CheckClient(ClientResultDto clientResultDto)
    {
        try
        {
            // 1. URL-кодирование параметров (важно для кириллицы и спецсимволов)
            var encodedFirstName = HttpUtility.UrlEncode(clientResultDto.firstName);
            var encodedLastName = HttpUtility.UrlEncode(clientResultDto.lastName);
            var encodedMiddleName = HttpUtility.UrlEncode(clientResultDto.middleName);

            // 2. Формируем URL с параметрами
            var url = $"/api/v1/client/is?" +
                     $"firstName={encodedFirstName}&" +
                     $"lastName={encodedLastName}&" +
                     $"middleName={encodedMiddleName}";

            _logger.LogDebug($"Sending GET request to: {url}");

            // 3. Отправляем GET запрос
            var response = await _httpClient.GetAsync(url);

            // 4. Проверяем статус ответа
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"HTTP Error: {response.StatusCode}");
                return false;
            }

            // 5. Читаем и возвращаем результат
            return await response.Content.ReadFromJsonAsync<bool>();
        }
        catch (HttpRequestException httpEx)
        {
            _logger.LogError(httpEx, "Network error while checking client");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while checking client");
            throw;
        }
    }
}