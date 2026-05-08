using System;
using System.Diagnostics;
using FakeWebShop.Persistence.Entities.Bugs;
using FakeWebShop.Persistence.MongoRepo_s.Interface_s;
using MongoDB.Driver;
using FakeWebShop.Persistence.MongoRepo_s.Options;
using Microsoft.Extensions.Options;
using FakeWebShop.Persistence.Constants;


namespace FakeWebShop.Persistence.MongoRepo_s;

public class DebugBugRepository : IDebugBugRepository
{
    private readonly IMongoCollection<DebugBug> _bugs;

    public DebugBugRepository(IMongoClient client, IOptions<MongoOptions> options)
    {
        var database = client.GetDatabase(options.Value.Database);
        _bugs = database.GetCollection<DebugBug>(MongoCollectionsNames.Bugs);
    }

    public async Task<Dictionary<string, bool>> GetAllBugsAsync() // geef alle bugs met huns statius
    {
        var bugs = await _bugs.Find(_ => true).ToListAsync();
        return bugs.ToDictionary(b => b.Key, b => b.Enabled);

    }

    public async Task<bool> GetBugStateAsync(string key) // geef true/false van 1 bug
    {
        var bug = await _bugs.Find(b => b.Key == key).FirstOrDefaultAsync();
        return bug?.Enabled ?? false;
    }

    public async Task UpdateBugAsync(string key, bool enabled) // zet een bug aan/uit, of maak ze aan als ze nog niet bestaat
    {
        var filter = Builders<DebugBug>.Filter.Eq(b => b.Key, key);
        var update = Builders<DebugBug>.Update
            .Set(b => b.Enabled, enabled)
            .SetOnInsert(b => b.Key, key);

        await _bugs.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });


    }
}
