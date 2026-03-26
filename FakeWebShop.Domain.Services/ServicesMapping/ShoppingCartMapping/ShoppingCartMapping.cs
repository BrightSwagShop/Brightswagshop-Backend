using System;
using FakeWebShop.Contracts.Request.CartRequest;
using FakeWebShop.Contracts.Response.CartResponse;
using FakeWebShop.Domain.Model.Cart;
using FakeWebShop.Persistence.Entities.Cart;

namespace FakeWebShop.Domain.Services.ServicesMapping.ShoppingCartMapping;

public static class ShoppingCartMapping
{
    // Request -> Model
    public static ShoppingCartModel AsModel(this ShoppingCartRequest request)
    {
        return new ShoppingCartModel
        {
            UserId = request.UserId,
            SessionId = request.SessionId,
            Items = request.Items.Select(i => new CartItemModel
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        };
    }

    // Model -> Entity
    public static ShoppingCart AsEntity(this ShoppingCartModel model)
    {
        return new ShoppingCart
        {
            Id = model.Id,
            UserId = model.UserId,
            SessionId = model.SessionId,
            UpdatedAt = model.UpdatedAt,
            TotalPrice = model.TotalPrice,
            SubTotal = model.SubTotal,
            Items = model.Items.Select(i => new CartItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList()
        };
    }

    // Entity -> Model
    public static ShoppingCartModel AsModel(this ShoppingCart entity)
    {
        return new ShoppingCartModel
        {
            Id = entity.Id,
            UserId = entity.UserId,
            SessionId = entity.SessionId,
            UpdatedAt = entity.UpdatedAt,
            Items = entity.Items.Select(i => new CartItemModel
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList(),
            TotalPrice = entity.TotalPrice,
            SubTotal = entity.SubTotal
        };
    }

    // Model -> Response
    public static ShoppingCartResponse AsResponse(this ShoppingCartModel model)
    {
        return new ShoppingCartResponse
        {
            Id = model.Id,
            UserId = model.UserId,
            SessionId = model.SessionId,
            UpdatedAt = model.UpdatedAt,
            TotalPrice = model.TotalPrice,
            SubTotal = model.SubTotal,
            Items = model.Items.Select(i => new CartItemResponse
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList()
        };
    }
}
