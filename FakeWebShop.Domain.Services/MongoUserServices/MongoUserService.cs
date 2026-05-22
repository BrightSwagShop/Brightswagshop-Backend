using FakeWebShop.Contracts.Request.UserRequest;
using FakeWebShop.Contracts.Response.UserResponse;
using FakeWebShop.Domain.Services.MongoServicesMapping.MongoUserMapping;
using FakeWebShop.Domain.Services.MongoUserServices.MongoInterfaces;
using FakeWebShop.Persistence.PublicUserRepo_s.MongoInterfaces;
using FakeWebShop.Domain.Services.Interface_s;

namespace FakeWebShop.Domain.Services.MongoUserServices;

public class MongoUserService(IMongoUserRepository repo, IDebugStateService debugService) : IMongoUserInterface
{
    public async Task<List<UserResponseContract>> GetAllAsync()
    {
        var users = await repo.GetAllAsync();
        var responses = users.Select(user => user.ToModel().ToResponse()).ToList();
        return await ApplyBrokenFavoritesAsync(responses);
    }

    public async Task<UserResponseContract?> Login(UserAuthRequestContract request)
    {

        if (await debugService.GetStateAsync("loginFails"))
            return null;

        var user = await repo.GetByUsernameAsync(request.Username);
        if (user == null)
            return null;

        var valid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        if (!valid)
            return null;

        return await ApplyBrokenFavoritesAsync(user.ToModel().ToResponse());
    }

    public async Task<UserResponseContract> Register(UserAuthRequestContract request)
    {
        var existing = await repo.GetByUsernameAsync(request.Username);
        if (existing != null)
            throw new Exception("User exists");

        var model = request.ToModel();
        model.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        model.Favorites = new List<string>();

        var entity = model.ToEntity();
        var created = await repo.CreateAsync(entity);

        return await ApplyBrokenFavoritesAsync(created.ToModel().ToResponse());
    }

    public async Task<UserResponseContract> RemoveFavoriteAsync(string userId, FavoriteRequestContract request)
    {
        var user = await repo.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("User not found");

        await repo.RemoveFavoriteAsync(user.Id, request.ProductId);

        var updatedUser = await repo.GetByIdAsync(userId);
        if (updatedUser == null)
            throw new Exception("Updated user not found");

        return await ApplyBrokenFavoritesAsync(updatedUser.ToModel().ToResponse());
    }

    public async Task<UserResponseContract> VoegFavoriteByUserAsync(string userId, FavoriteRequestContract request)
    {
        var user = await repo.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("User not found");

        await repo.VoegFavoriteByUserAsync(user.Id, request.ProductId);

        var updatedUser = await repo.GetByIdAsync(userId);
        if (updatedUser == null)
            throw new Exception("Updated user not found");

        return await ApplyBrokenFavoritesAsync(updatedUser.ToModel().ToResponse());
    }

    public async Task<UserResponseContract> GetByIdAsync(string userId)
    {
        var user = await repo.GetByIdAsync(userId);
        if (user == null)
            throw new Exception("User not found");

        return await ApplyBrokenFavoritesAsync(user.ToModel().ToResponse());
    }

    private async Task<UserResponseContract> ApplyBrokenFavoritesAsync(UserResponseContract user)
    {
        if (await debugService.GetStateAsync("brokenFavorites"))
        {
            user.Favorites = new List<string>();
        }

        return user;
    }

    private async Task<List<UserResponseContract>> ApplyBrokenFavoritesAsync(List<UserResponseContract> users)
    {
        if (await debugService.GetStateAsync("brokenFavorites"))
        {
            foreach (var user in users)
            {
                user.Favorites = new List<string>();
            }
        }

        return users;
    }
}


