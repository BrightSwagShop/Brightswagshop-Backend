using System;
using FakeWebShop.Contracts.Request.CartRequest;
using FakeWebShop.Contracts.Response.CartResponse;

namespace FakeWebShop.Domain.Services.Interface_s;

public interface IShoppingCartService
{
    Task<ShoppingCartResponse> CreateAsync(ShoppingCartRequest request);
    Task<ShoppingCartResponse?> GetByUserIdAsync(string userId);
    Task<ShoppingCartResponse?> GetBySessionIdAsync(string sessionId);
    Task<bool> DeleteAsync(string id);
    Task<ShoppingCartResponse?> ApplyDiscountCodeAsync(string cartId, string code);
}
