
using FakeWebShop.Domain.Services.Interface_s;
using FakeWebShop.Persistence.MongoRepo_s.Interface_s;

namespace FakeWebShop.Domain.Services;

public class DebugStateService(IDebugBugRepository repo) : IDebugStateService
{
    public async Task<Dictionary<string, bool>> GetAllStatesAsync()
    {
        return await repo.GetAllBugsAsync();
    }

    public async Task<bool> GetStateAsync(string key)
    {
        return await repo.GetBugStateAsync(key);
    }

    public async Task SetStateAsync(string key, bool value)
    {
        await repo.UpdateBugAsync(key, value);

    }
}
