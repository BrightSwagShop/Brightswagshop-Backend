using System;
using FakeWebShop.Contracts.Request;
using FakeWebShop.Contracts.Request.Products.BaseProductRequest;
using FakeWebShop.Contracts.Response.Products.BaseProductResponse;
using FakeWebShop.Domain.Model;
using FakeWebShop.Domain.Models;
using FakeWebShop.Persistence.Entities;
using FakeWebShop.Persistence.Entities.BaseProduct;

namespace FakeWebShop.Domain.Services.MongoServicesMapping;

internal static class MongoProductMapping
{
    // Create & Put         Get 
    // Request -> Model -> Entity -> Model -> Response
    // 1 Entry mapper kijkt op runtime welk subtype het is (switch pattern), 

    // Request naar model
    public static ProductModel AsModel(this MongoProductRequest request) =>
    request switch
    {
        ClothingProductRequest clothing => clothing.AsModel(),
        MugProductRequest mug => mug.AsModel(),
        
        _ => throw new NotSupportedException($"Unknown product request type: {request.GetType().Name}")
    };

    // Model naar Entity
    public static Product AsEntity(this ProductModel model) =>
    model switch
    {

        ClothingProductModel clothing => clothing.AsEntity(),
        MugProductModel mug => mug.AsEntity(),
        _ => throw new NotSupportedException($"Unknown product model type: {model.GetType().Name}")
    };

    // Entity naar Model
    public static ProductModel AsModel(this Product entity) =>
    entity switch
    {
        ClothingProduct clothing => clothing.AsModel(),
        MugProduct mug => mug.AsModel(),
        _ => throw new NotSupportedException($"Unknown product entity type: {entity.GetType().Name}")
    };

    // Model naar Response
    public static MongoProductResponse AsResponse(this ProductModel model) =>
    model switch
    {

        ClothingProductModel clothing => clothing.AsResponse(),
        MugProductModel mug => mug.AsResponse(),
        _ => throw new NotSupportedException($"Unknown product model type: {model.GetType().Name}")
    };
}
