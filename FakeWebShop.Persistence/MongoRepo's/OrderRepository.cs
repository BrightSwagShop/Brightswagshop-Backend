using System;
using FakeWebShop.Persistence.Constants;
using FakeWebShop.Persistence.Entities.Order;
using FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;
using FakeWebShop.Persistence.MongoRepo_s.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FakeWebShop.Persistence.MongoRepo_s;

public class OrderRepository : IOrderRepository
{
    private readonly IMongoCollection<Order> _orders;

    public OrderRepository(IMongoClient client, IOptions<MongoOptions> options)
    {
        var database = client.GetDatabase(options.Value.Database);
        _orders = database.GetCollection<Order>(MongoCollectionsNames.Orders);
    }
    public async Task CreateAsync(Order order)
    {
        await _orders.InsertOneAsync(order);
    }

    public async Task<Order?> GetByIdAsync(string id)
    {
        return await _orders
            .Find(order => order.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Order>> GetByUserIdAsync(string userId)
    {
        return await _orders
            .Find(order => order.UserId == userId)
            .ToListAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        await _orders.ReplaceOneAsync(
            existingOrder => existingOrder.Id == order.Id,
            order);
    }
}
