using FakeWebShop.Contracts.Request;
using FakeWebShop.Contracts.Response;
using FakeWebShop.Contracts.Response.VariantResponse;
using FakeWebShop.Domain.Model;
using FakeWebShop.Domain.Model.VariantModel;
using FakeWebShop.Persistence.Entities;
using FakeWebShop.Persistence.Entities.Variant;



namespace FakeWebShop.Domain.Services.MongoServicesMapping.ProductTypesMapping;

internal static class MugProductMapping
{
    // Create & Put           // Get
    // Request -> Model -> Entity -> Model -> Response
    // Request -> Model
    public static MugProductModel AsModel(this MugProductRequest request)
    {
        return new MugProductModel
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = request.Category,
            ProductType = request.ProductType,
            IsActive = request.IsActive,
            Kleuren = request.Kleuren.Select(k => new ColorVariantModel
            {
                Kleur = k.Kleur,
                ImageUrl = k.ImageUrl,
                Stock = k.Stock,
                Sku = k.Sku
            }).ToList()
        };
    }

    // Model -> Entity
    public static MugProduct AsEntity(this MugProductModel model)
    {
        return new MugProduct
        {
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            Category = model.Category,
            ProductType = model.ProductType,
            IsActive = model.IsActive,
            Kleuren = model.Kleuren.Select(k => new ColorVariant
            {
                Kleur = k.Kleur,
                ImageUrl = k.ImageUrl,
                Stock = k.Stock,
                Sku = k.Sku
            }).ToList()
        };
    }

    // Entity -> Model
    public static MugProductModel AsModel(this MugProduct entity)
    {
        return new MugProductModel
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Price = entity.Price,
            Category = entity.Category,
            ProductType = entity.ProductType,
            IsActive = entity.IsActive,
            Kleuren = entity.Kleuren.Select(k => new ColorVariantModel
            {
                Kleur = k.Kleur,
                ImageUrl = k.ImageUrl,
                Stock = k.Stock,
                Sku = k.Sku
            }).ToList()
        };
    }


    // Model -> Response
    public static MugProductResponse AsResponse(this MugProductModel model)
    {
        return new MugProductResponse
        {
            Id = model.Id,
            Name = model.Name,
            Description = model.Description,
            Price = model.Price,
            Category = model.Category,
            ProductType = model.ProductType,
            IsActive = model.IsActive,
            Kleuren = model.Kleuren.Select(k => new ColorVariantResponse
            {
                Kleur = k.Kleur,
                ImageUrl = k.ImageUrl,
                Stock = k.Stock,
                Sku = k.Sku
            }).ToList()
        };
    }


}
