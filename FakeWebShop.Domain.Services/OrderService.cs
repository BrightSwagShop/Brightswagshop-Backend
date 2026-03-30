using System;
using FakeWebShop.Contracts.Request.OrderRequest;
using FakeWebShop.Contracts.Response.OrderResponse;
using FakeWebShop.Domain.Enums;
using FakeWebShop.Domain.Model.Order;
using FakeWebShop.Domain.Services.Interface_s;
using FakeWebShop.Domain.Services.ServicesMapping.OrderMapping;
using FakeWebShop.Domain.Services.ServicesMapping.ShoppingCartMapping;
using FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;

namespace FakeWebShop.Domain.Services;

public class OrderService(IOrderRepository orderRepo, IMongoProductRepository productRepo, IShoppingCartRepository cartRepo) : IOrderService
{
    public async Task<OrderResponse> CreateAsync(OrderRequest request)
    {
        var orderModel = new OrderModel
        {
            UserId = request.UserId,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatusEnum.Pending,
            PaymentStatus = PaymentStatusEnum.Pending
        };

        foreach (var item in request.Items)
        {
            var product = await productRepo.GetByIdAsync(item.ProductId);

            if (product is null)
                throw new Exception($"Product with id {item.ProductId} not found.");

            orderModel.Items.Add(new OrderItemModel
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = item.Quantity
            });
        }

        orderModel.TotalPrice = orderModel.Items.Sum(i => i.SubTotal);

        var entity = orderModel.AsEntity();

        await orderRepo.CreateAsync(entity);

        return entity.AsModel().AsResponse();
    }

    public async Task<OrderResponse?> GetByIdAsync(string id)
    {
        var entity = await orderRepo.GetByIdAsync(id);

        if (entity is null)
            return null;

        return entity.AsModel().AsResponse();
    }

    public async Task<List<OrderResponse>> GetByUserIdAsync(string userId)
    {
        var entities = await orderRepo.GetByUserIdAsync(userId);

        return entities
            .Select(entity => entity.AsModel().AsResponse())
            .ToList();
    }

    public async Task UpdateAsync(string id, OrderRequest request)
    {
        var existingEntity = await orderRepo.GetByIdAsync(id);

        if (existingEntity is null)
            throw new Exception("Order not found.");

        var model = request.AsModel();

        model.Id = existingEntity.Id;
        model.CreatedAt = existingEntity.CreatedAt;
        model.Status = existingEntity.Status;
        model.PaymentStatus = existingEntity.PaymentStatus;
        model.TotalPrice = model.Items.Sum(item => item.SubTotal);

        var updatedEntity = model.AsEntity();

        await orderRepo.UpdateAsync(updatedEntity);
    }


    // Payment 
    public async Task UpdatePaymentStatusAsync(string id, PaymentStatusEnum status, string? sessionId)
    {
        var entity = await orderRepo.GetByIdAsync(id);

        if (entity is null)
            throw new Exception("Order not found.");

        entity.PaymentStatus = status;
        entity.StripeCheckoutSessionId = sessionId;

        await orderRepo.UpdateAsync(entity);
    }

    public async Task SetStripeCheckoutSessionIdAsync(string id, string sessionId)
    {
        var entity = await orderRepo.GetByIdAsync(id);

        if (entity is null)
            throw new Exception("Order not found.");

        entity.StripeCheckoutSessionId = sessionId;

        await orderRepo.UpdateAsync(entity);
    }

    public async Task<OrderResponse> CreateFromCartAsync(string userId)
    {
        var cartEntity = await cartRepo.GetByUserIdAsync(userId);

        if (cartEntity is null)
            throw new Exception($"Shopping cart for user {userId} not found.");

        var cartModel = cartEntity.AsModel();

        if (cartModel.Items is null || cartModel.Items.Count == 0)
            throw new Exception("Shopping cart is empty.");

        var orderModel = new OrderModel
        {
            UserId = cartModel.UserId,
            Status = OrderStatusEnum.Pending,
            CreatedAt = DateTime.UtcNow,
            Items = cartModel.Items.Select(item => new OrderItemModel
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity

            }).ToList()
        };

        orderModel.TotalPrice = orderModel.Items.Sum(i => i.UnitPrice * i.Quantity);

        var orderEntity = orderModel.AsEntity();

        await orderRepo.CreateAsync(orderEntity);

        return orderEntity.AsModel().AsResponse();
    }
}
