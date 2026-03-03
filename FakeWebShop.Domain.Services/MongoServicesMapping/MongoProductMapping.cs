using System;
using FakeWebShop.Contracts.Request;
using FakeWebShop.Contracts.Request.Products.BaseProductRequest;
using FakeWebShop.Contracts.Response.Products.BaseProductResponse;
using FakeWebShop.Domain.Model;
using FakeWebShop.Domain.Models;
using FakeWebShop.Domain.Services.MongoServicesMapping.ProductTypesMapping; // ✅ juiste namespace
using FakeWebShop.Persistence.Entities;
using FakeWebShop.Persistence.Entities.BaseProduct;

namespace FakeWebShop.Domain.Services.MongoServicesMapping;

internal static class MongoProductMapping
{
    // Request -> Model
    public static ProductModel ToModel(this MongoProductRequest request) =>
        request switch
        {
            ClothingProductRequest clothing => clothing.AsModel(),
            MugProductRequest mug => mug.AsModel(),
            _ => throw new NotSupportedException($"Unknown product request type: {request.GetType().Name}")
        };

    // Model -> Entity
    public static Product ToEntity(this ProductModel model) =>
        model switch
        {
            ClothingProductModel clothing => clothing.AsEntity(),
            MugProductModel mug => mug.AsEntity(),
            _ => throw new NotSupportedException($"Unknown product model type: {model.GetType().Name}")
        };

    // Entity -> Model
    public static ProductModel ToModel(this Product entity) =>
        entity switch
        {
            ClothingProduct clothing => clothing.AsModel(),
            MugProduct mug => mug.AsModel(),
            _ => throw new NotSupportedException($"Unknown product entity type: {entity.GetType().Name}")
        };

    // Model -> Response
    public static MongoProductResponse ToResponse(this ProductModel model) =>
        model switch
        {
            ClothingProductModel clothing => clothing.AsResponse(),
            MugProductModel mug => mug.AsResponse(),
            _ => throw new NotSupportedException($"Unknown product model type: {model.GetType().Name}")
        };
}