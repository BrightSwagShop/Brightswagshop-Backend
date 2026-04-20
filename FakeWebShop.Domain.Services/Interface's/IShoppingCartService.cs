using System;
using FakeWebShop.Contracts.Request.CartRequest;
using FakeWebShop.Contracts.Response.CartResponse;

namespace FakeWebShop.Domain.Services.Interface_s;

public interface IShoppingCartService
{
    Task<ShoppingCartResponse> CreateAsync(ShoppingCartRequest request);
    Task<ShoppingCartResponse?> GetByUserIdAsync(string userId);
    Task<ShoppingCartResponse?> GetBySessionIdAsync(string sessionId);
    Task<ShoppingCartResponse?> UpdateQuantityAsync(string userId, CartItemRequest request);


    Task<bool> DeleteAsync(string id); // Delete heel de ShoppingCart
    Task<ShoppingCartResponse?> RemoveItemAsync(string userId, CartItemRequest request); // Delete 1 item in de Shopping cart
    Task<ShoppingCartResponse> AddItemAsync(string userId, CartItemRequest request); // Item toevoegen aan de shoppingCart
    Task<ShoppingCartResponse?> ApplyDiscountCodeAsync(string cartId, string code);
}
