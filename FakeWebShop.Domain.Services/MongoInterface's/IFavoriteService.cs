using System;
using FakeWebShop.Contracts.Response;
using FakeWebShop.Domain.Model.favoritesModel;

namespace FakeWebShop.Domain.Services.MongoInterface_s;

public interface IFavoriteService
{
     Task AddFavorite(string userId, string productId);

    Task RemoveFavorite(string userId, string productId);

    Task<List<FavoriteResponseContract>> GetFavorites(string userId);

}
