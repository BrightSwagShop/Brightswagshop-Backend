using System;
using FakeWebShop.Contracts.Request;
using FakeWebShop.Contracts.Response;
using FakeWebShop.Contracts.Response.VariantResponse;
using FakeWebShop.Domain.Model;
using FakeWebShop.Domain.Model.VariantModel;
using FakeWebShop.Persistence.Entities;
using FakeWebShop.Persistence.Entities.Variant;

namespace FakeWebShop.Domain.Services.MongoServicesMapping.ProductTypesMapping;

internal static class ClothingProductMapping
{
    // Create & Put           // Get
    // Request -> Model -> Entity -> Model -> Response

    // Request naar Model
    public static ClothingProductModel AsModel(this ClothingProductRequest request)
    {
        return new ClothingProductModel
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
    public static ClothingProduct AsEntity(this ClothingProductModel model)
    {
        return new ClothingProduct
        {
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
    public static ClothingProductModel AsModel(this ClothingProduct entity)
    {
        return new ClothingProductModel
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
    public static ClothingProductResponse AsResponse(this ClothingProductModel model)
    {
        return new ClothingProductResponse
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
