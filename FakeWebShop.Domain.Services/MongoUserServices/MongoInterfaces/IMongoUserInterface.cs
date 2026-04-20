using System;
using FakeWebShop.Contracts.Request.UserRequest;
using FakeWebShop.Contracts.Response.UserResponse;


namespace FakeWebShop.Domain.Services.MongoUserServices.MongoInterfaces;

public interface IMongoUserInterface
{
    Task<UserResponseContract> Register(UserAuthRequestContract request);
    Task<UserResponseContract?> Login(UserAuthRequestContract request);
    Task<UserResponseContract> VoegFavoriteByUserAsync(string userId, FavoriteRequestContract request);
    Task<UserResponseContract> RemoveFavoriteAsync(string userId, FavoriteRequestContract request);
    Task<UserResponseContract> GetByIdAsync(string userId);

}
