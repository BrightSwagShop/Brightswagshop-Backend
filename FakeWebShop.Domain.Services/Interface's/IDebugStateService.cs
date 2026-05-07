using System;

namespace FakeWebShop.Domain.Services.Interface_s;

public interface IDebugStateService
{
    Task<Dictionary<string, bool>> GetAllStatesAsync();
    Task<bool> GetStateAsync(string key);
    Task SetStateAsync(string key, bool value);
}
