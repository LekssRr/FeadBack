using FeadBack.controller.dto;

namespace FeadBack.service;

public interface IClientCheckService
{
    Task<bool> CheckClient(ClientResultDto  clientResultDto);
}