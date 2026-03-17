using System;
using FakeWebShop.Persistence.Entities.Order;

namespace FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(string id);
    Task<List<Order>> GetByUserIdAsync(string userId);
    Task CreateAsync(Order order);
    Task UpdateAsync(Order order);
}
