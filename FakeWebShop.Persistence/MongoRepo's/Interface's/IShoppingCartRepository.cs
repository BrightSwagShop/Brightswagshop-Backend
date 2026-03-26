using System;
using FakeWebShop.Persistence.Entities.Cart;

namespace FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;

public interface IShoppingCartRepository
{
    Task<ShoppingCart?> GetByUserIdAsync(string userId);
    Task<ShoppingCart?> GetBySessionIdAsync(string sessionId);
    Task CreateAsync(ShoppingCart cart);
    Task<bool> DeleteAsync(string id);
    // Task UpdateAsync(ShoppingCart cart);
}
