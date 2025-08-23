namespace FeadBack.service;
using FeadBack.controller.dto;

public interface IVinCheckService
{
    Task<AutoResultDto> autoVinAsync(string vin);
}