using System;
using FakeWebShop.Contracts.Request.OrderRequest;
using FakeWebShop.Contracts.Response.OrderResponse;
using FakeWebShop.Domain.Model.Order;
using FakeWebShop.Persistence.Entities.Order;

namespace FakeWebShop.Domain.Services.ServicesMapping.OrderMapping;

public static class OrderMapping
{
    // Request -> Model
    public static OrderModel AsModel(this OrderRequest request)
    {
        return new OrderModel
        {
            UserId = request.UserId,
            Items = request.Items.Select(i => new OrderItemModel
            {
                ProductId = i.ProductId,
                Quantity = i.Quantity
            }).ToList()
        };
    }

    // Model -> Entity
    public static Order AsEntity(this OrderModel model)
    {
        return new Order
        {
            Id = model.Id,
            UserId = model.UserId,
            StripeCheckoutSessionId = model.StripeCheckoutSessionId,
            Status = model.Status,
            PaymentStatus = model.PaymentStatus,
            CreatedAt = model.CreatedAt,
            TotalPrice = model.TotalPrice,
            Items = model.Items.Select(i => new OrderItem
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList()
        };
    }

    // Entity -> Model
    public static OrderModel AsModel(this Order entity)
    {
        return new OrderModel
        {
            Id = entity.Id,
            UserId = entity.UserId,
            StripeCheckoutSessionId = entity.StripeCheckoutSessionId,
            Status = entity.Status,
            PaymentStatus = entity.PaymentStatus,
            CreatedAt = entity.CreatedAt,
            TotalPrice = entity.TotalPrice,
            Items = entity.Items.Select(i => new OrderItemModel
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity
            }).ToList()
        };
    }

    // Model -> Response
    public static OrderResponse AsResponse(this OrderModel model)
    {
        return new OrderResponse
        {
            Id = model.Id,
            UserId = model.UserId,
            StripeCheckoutSessionId = model.StripeCheckoutSessionId,
            Status = model.Status,
            PaymentStatus = model.PaymentStatus,
            CreatedAt = model.CreatedAt,
            TotalPrice = model.TotalPrice,
            Items = model.Items.Select(i => new OrderItemResponse
            {
                ProductId = i.ProductId,
                ProductName = i.ProductName,
                UnitPrice = i.UnitPrice,
                Quantity = i.Quantity,
                SubTotal = i.SubTotal
            }).ToList()
        };
    }

}
