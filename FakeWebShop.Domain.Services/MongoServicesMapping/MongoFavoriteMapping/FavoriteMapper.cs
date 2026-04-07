using System;
using FakeWebShop.Domain.Model.favoritesModel;
using FakeWebShop.Persistence.Entities.Favorites;
using MongoDB.Bson;

namespace FakeWebShop.Domain.Services.MongoServicesMapping.MongoFavoriteMapping;

public static class FavoriteMapper
{
    public static Favorite ToDomain(FavoriteEntity entity)
    {
        return new Favorite
        {
            Id = entity.Id,
            UserId = entity.UserId,
            ProductId = entity.ProductId,
            CreatedAt = entity.CreatedAt
        };
    }

    public static FavoriteEntity ToEntity(Favorite model)
    {
        return new FavoriteEntity
        {
            Id = string.IsNullOrEmpty(model.Id)
                ? ObjectId.GenerateNewId().ToString()
                : model.Id,

            UserId = model.UserId,
            ProductId = model.ProductId,
            CreatedAt = model.CreatedAt
        };
    }

}
