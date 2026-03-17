using System;
using FakeWebShop.Persistence.Constants;
using FakeWebShop.Persistence.Entities.Cart;
using FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;
using FakeWebShop.Persistence.MongoRepo_s.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FakeWebShop.Persistence.MongoRepo_s;

public class ShoppingCartRepository : IShoppingCartRepository
{
    private readonly IMongoCollection<ShoppingCart> _shoppingCarts;

    public ShoppingCartRepository(IMongoClient client, IOptions<MongoOptions> options)
    {
        var database = client.GetDatabase(options.Value.Database);
        _shoppingCarts = database.GetCollection<ShoppingCart>(MongoCollectionsNames.ShoppingCarts);
    }

    public async Task<ShoppingCart?> GetByUserIdAsync(string userId)
    {
        return await _shoppingCarts
            .Find(cart => cart.UserId == userId)
            .FirstOrDefaultAsync();
    }
    public async Task<ShoppingCart?> GetBySessionIdAsync(string sessionId)
    {
        return await _shoppingCarts
            .Find(cart => cart.SessionId == sessionId)
            .FirstOrDefaultAsync();
    }
    public async Task CreateAsync(ShoppingCart cart)
    {
        await _shoppingCarts.InsertOneAsync(cart);
    }

    public async Task<bool> DeleteAsync(string id)
    {
        var deleteResult = await _shoppingCarts.DeleteOneAsync(cart => cart.Id == id);
        return deleteResult.DeletedCount > 0;
    }


}
