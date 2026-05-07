using System;
using FakeWebShop.Contracts.Request;
using FakeWebShop.Contracts.Response;
using FakeWebShop.Contracts.Response.VariantResponse;
using FakeWebShop.Domain.Model;
using FakeWebShop.Domain.Model.VariantModel;
using FakeWebShop.Persistence.Entities;
using FakeWebShop.Persistence.Entities.Variant;

namespace FakeWebShop.Domain.Services.MongoServicesMapping.ProductTypesMapping;

internal static class ProductWithSizesMapping
{
    // Create & Put           // Get
    // Request -> Model -> Entity -> Model -> Response

    // Request naar Model
    public static ProductWithSizesModel AsModel(this ProductWithSizesRequest request)
    {
        return new ProductWithSizesModel
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category,
            ProductType = request.ProductType,
            IsActive = request.IsActive,
            Kleuren = request.Kleuren.Select(k => new ColorVariantClothesModel
            {
                Kleur = k.Kleur,
                ImageUrl = k.ImageUrl,
                Maten = k.Maten.Select(m => new SizeVariantModel
                {
                    Maat = m.Maat,
                    Stock = m.Stock,
                    Sku = m.Sku
                }).ToList()
            }).ToList()
        };

    }

    // Model naar Entity
    public static ProductWithSizes AsEntity(this ProductWithSizesModel model)
    {
        return new ProductWithSizes
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            Category = model.Category,
            ProductType = model.ProductType,
            IsActive = model.IsActive,
            Kleuren = model.Kleuren.Select(k => new ColorVariantClothes
            {
                Kleur = k.Kleur,
                ImageUrl = k.ImageUrl,
                Maten = k.Maten.Select(m => new SizeVariant
                {
                    Maat = m.Maat,
                    Stock = m.Stock,
                    Sku = m.Sku
                }).ToList()
            }).ToList()
        };
    }

    // Entity Naar Model
    public static ProductWithSizesModel AsModel(this ProductWithSizes entity)
    {
        return new ProductWithSizesModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            Category = entity.Category,
            ProductType = entity.ProductType,
            IsActive = entity.IsActive,
            Kleuren = entity.Kleuren.Select(k => new ColorVariantClothesModel
            {
                Kleur = k.Kleur,
                ImageUrl = k.ImageUrl,
                Maten = k.Maten.Select(m => new SizeVariantModel
                {
                    Maat = m.Maat,
                    Stock = m.Stock,
                    Sku = m.Sku
                }).ToList()
            }).ToList()
        };
    }

    // Model naar Response
    public static ProductWithSizesResponse AsResponse(this ProductWithSizesModel model)
    {
        return new ProductWithSizesResponse
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            Category = model.Category,
            ProductType = model.ProductType,
            IsActive = model.IsActive,
            Kleuren = model.Kleuren.Select(k => new ColorVariantClothesResponse
            {
                Kleur = k.Kleur,
                ImageUrl = k.ImageUrl,
                Maten = k.Maten.Select(m => new SizeVariantResponse
                {
                    Maat = m.Maat,
                    Stock = m.Stock,
                    Sku = m.Sku
                }).ToList()
            }).ToList()
        };
    }
}
