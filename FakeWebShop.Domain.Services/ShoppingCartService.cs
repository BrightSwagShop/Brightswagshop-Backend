using System;
using FakeWebShop.Contracts.Request.CartRequest;
using FakeWebShop.Contracts.Response.CartResponse;
using FakeWebShop.Domain.Model.Cart;
using FakeWebShop.Domain.Services.Interface_s;
using FakeWebShop.Domain.Services.ServicesMapping.ShoppingCartMapping;
using FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;

namespace FakeWebShop.Domain.Services;

public class ShoppingCartService(IShoppingCartRepository cartRepo, IMongoProductRepository productRepo) : IShoppingCartService
{
    public async Task<ShoppingCartResponse> CreateAsync(ShoppingCartRequest request)
    {
        var cartModel = new ShoppingCartModel
        {
            UserId = request.UserId,
            SessionId = request.SessionId,
            UpdatedAt = DateTime.UtcNow
        };

        foreach (var item in request.Items)
        {
            var product = await productRepo.GetByIdAsync(item.ProductId);

            if (product is null)
                throw new Exception($"Product with id {item.ProductId} not found.");

            cartModel.Items.Add(new CartItemModel
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = item.Quantity
            });
        }

        cartModel.TotalPrice = cartModel.Items.Sum(i => i.UnitPrice * i.Quantity);

        var entity = cartModel.AsEntity();

        await cartRepo.CreateAsync(entity);

        return entity.AsModel().AsResponse();
    }

    public async Task<bool> DeleteAsync(string id)
    {
        return await cartRepo.DeleteAsync(id);
    }

    public async Task<ShoppingCartResponse?> GetBySessionIdAsync(string sessionId)
    {
        var entity = await cartRepo.GetBySessionIdAsync(sessionId);

        if (entity is null)
            return null;

        return entity.AsModel().AsResponse();
    }

    public async Task<ShoppingCartResponse?> GetByUserIdAsync(string userId)
    {
        var entity = await cartRepo.GetByUserIdAsync(userId);

        if (entity is null)
            return null;

        return entity.AsModel().AsResponse();
    }
}
