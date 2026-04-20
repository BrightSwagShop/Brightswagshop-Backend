using System;
using FakeWebShop.Persistence.Constants;
using FakeWebShop.Persistence.Entities.Discount;
using FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;
using FakeWebShop.Persistence.MongoRepo_s.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FakeWebShop.Persistence.MongoRepo_s;

public class DiscountRepository : IDiscountRepository
{
    private readonly IMongoCollection<Discount> _discounts;

    public DiscountRepository(IMongoClient client, IOptions<MongoOptions> options)
    {
        var database = client.GetDatabase(options.Value.Database);
        _discounts = database.GetCollection<Discount>(MongoCollectionsNames.Discounts);
    }

    public async Task<Discount> CreateAsync(Discount discount)
    {
        await _discounts.InsertOneAsync(discount);
        return discount;
    }

    public async Task<Discount?> GetByIdAsync(string id)
    {
        return await _discounts
            .Find(discount => discount.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<Discount?> GetByCodeAsync(string code)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        return await _discounts.Find(d => d.Code == code).FirstOrDefaultAsync();

    }

    public async Task DeleteAsync(string id)
    {
        await _discounts.DeleteOneAsync(discount => discount.Id == id);
    }

    public async Task<List<Discount>> GetAllAsync()
    {
        return await _discounts.Find(_ => true).ToListAsync();
    }

    public async Task UpdateAsync(Discount discount)
    {
        await _discounts.ReplaceOneAsync(d => d.Id == discount.Id, discount);
    }
}
