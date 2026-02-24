using System;
using FakeWebShop.Contracts;
using FakeWebShop.Domain.Model;
using FakeWebShop.Persistence.Entities.Model;

namespace FakeWebShop.Domain.Services.Mapping;

public static class ProductMappingExtensions
{
     public static void MapBaseToEntity(ProductModel model, Product entity)
    {
        entity.Naam = model.Naam!;
        entity.Beschrijving = model.Beschrijving!;
        entity.Prijs = model.Prijs;
        entity.ImageUrl = model.ImageUrl!;
        entity.CategoryId = model.CategoryId;
    }

    public static void MapBaseToModel(Product entity, ProductModel model)
    {
        model.Id = entity.Id;
        model.Naam = entity.Naam;
        model.Beschrijving = entity.Beschrijving;
        model.Prijs = entity.Prijs;
        model.ImageUrl = entity.ImageUrl;
        model.CategoryId = entity.CategoryId;
    }
      public static ProductResponseContract AsContract(this Product entity)
    {
        return new ProductResponseContract
        {
            Id = entity.Id,
            Naam = entity.Naam ?? throw new Exception(),
            Beschrijving = entity.Beschrijving ?? throw new Exception(),
            Prijs = entity.Prijs,
            ImageUrl = entity.ImageUrl ?? throw new Exception(),
            CategoryId = entity.CategoryId
        };
    }

}
