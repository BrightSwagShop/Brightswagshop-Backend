using System;

namespace FakeWebShop.Persistence.MongoRepo_s.Interface_s;

public interface IDebugBugRepository
{
    Task<Dictionary<string, bool>> GetAllBugsAsync();
    Task<bool> GetBugStateAsync(string key);
    Task UpdateBugAsync(string key, bool enabled);
}
