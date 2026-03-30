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
                SelectedColor = i.SelectedColor,
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
            Items = model.Items.Select(i => new CartItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                SelectedColor = i.SelectedColor,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                ImageUrl = i.ImageUrl
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
            TotalPrice = entity.TotalPrice,
            Items = entity.Items.Select(i => new CartItemModel
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                SelectedColor = i.SelectedColor,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                ImageUrl = i.ImageUrl
            }).ToList()
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
            Items = model.Items.Select(i => new CartItemResponse
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                SelectedColor = i.SelectedColor,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                ImageUrl = i.ImageUrl
            }).ToList()
        };
    }
}
