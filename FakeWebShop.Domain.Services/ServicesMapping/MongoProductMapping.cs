using FakeWebShop.Contracts.Request;
using FakeWebShop.Contracts.Request.Products.BaseProductRequest;
using FakeWebShop.Contracts.Response.Products.BaseProductResponse;
using FakeWebShop.Domain.Enums;
using FakeWebShop.Domain.Model;
using FakeWebShop.Domain.Services.MongoServicesMapping.ProductTypesMapping;
using FakeWebShop.Persistence.Entities;
using FakeWebShop.Persistence.Entities.BaseProduct;

namespace FakeWebShop.Domain.Services.MongoServicesMapping;

internal static class MongoProductMapping
{
    // Request -> Model
    public static ProductModel ToModel(this MongoProductRequest request)
    {
        ValidateProductType(request);

        return request switch
        {
            ProductWithSizesRequest product => product.AsModel(),
            SimpleProductRequest product => product.AsModel(),
            _ => throw new NotSupportedException($"Unknown product request type: {request.GetType().Name}")
        };
    }
    // Model -> Entity
    public static Product ToEntity(this ProductModel model) =>
        model switch
        {
            ProductWithSizesModel clothing => clothing.AsEntity(),
            SimpleProductModel mug => mug.AsEntity(),
            _ => throw new NotSupportedException($"Unknown product model type: {model.GetType().Name}")
        };

    // Entity -> Model
    public static ProductModel ToModel(this Product entity) =>
        entity switch
        {
            ProductWithSizes clothing => clothing.AsModel(),
            SimpleProduct mug => mug.AsModel(),
            _ => throw new NotSupportedException($"Unknown product entity type: {entity.GetType().Name}")
        };

    // Model -> Response
    public static MongoProductResponse ToResponse(this ProductModel model) =>
        model switch
        {
            ProductWithSizesModel clothing => clothing.AsResponse(),
            SimpleProductModel mug => mug.AsResponse(),
            _ => throw new NotSupportedException($"Unknown product model type: {model.GetType().Name}")
        };


    // Validatie van ProductType
    private static void ValidateProductType(MongoProductRequest request)
    {
        if (request is ProductWithSizesRequest &&
            request.ProductType is not
            ProductTypeEnum.TShirt and not
            ProductTypeEnum.Hoodie and not
            ProductTypeEnum.Sportkledij and not
            ProductTypeEnum.Sokken)
        {
            throw new ArgumentException($"ProductType {request.ProductType} is not valid for products with sizes.");
        }

        if (request is SimpleProductRequest &&
            request.ProductType is not
            ProductTypeEnum.Drinkfles and not
            ProductTypeEnum.Mok and not
            ProductTypeEnum.Onderlegger and not
            ProductTypeEnum.Balpen and not
            ProductTypeEnum.Eendje)
        {
            throw new ArgumentException($"ProductType {request.ProductType} is not valid for simple products.");
        }
    }
}