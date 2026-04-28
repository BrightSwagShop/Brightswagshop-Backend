using FakeWebShop.Contracts.Request;
using FakeWebShop.Contracts.Response;
using FakeWebShop.Contracts.Response.VariantResponse;
using FakeWebShop.Domain.Model;
using FakeWebShop.Domain.Model.VariantModel;
using FakeWebShop.Persistence.Entities;
using FakeWebShop.Persistence.Entities.Variant;



namespace FakeWebShop.Domain.Services.MongoServicesMapping.ProductTypesMapping;

internal static class SimpleProductMapping
{
    // Create & Put           // Get
    // Request -> Model -> Entity -> Model -> Response
    // Request -> Model
    public static SimpleProductModel AsModel(this SimpleProductRequest request)
    {
        return new SimpleProductModel
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
    public static SimpleProduct AsEntity(this SimpleProductModel model)
    {
        return new SimpleProduct
        {
            Id = model.Id,
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
    public static SimpleProductModel AsModel(this SimpleProduct entity)
    {
        return new SimpleProductModel
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
    public static SimpleProductResponse AsResponse(this SimpleProductModel model)
    {
        return new SimpleProductResponse
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
