using System;
using FakeWebShop.Contracts.Request.OrderRequest;
using FakeWebShop.Contracts.Response.OrderResponse;
using FakeWebShop.Domain.Enums;

namespace FakeWebShop.Domain.Services.Interface_s;

public interface IOrderService
{
    Task<OrderResponse> CreateAsync(OrderRequest request);
    Task<OrderResponse?> GetByIdAsync(string id);
    Task<List<OrderResponse>> GetByUserIdAsync(string userId);
    Task UpdateAsync(string id, OrderRequest request);

    Task UpdatePaymentStatusAsync(string id, PaymentStatusEnum status, string? sessionId);
    Task SetStripeCheckoutSessionIdAsync(string id, string sessionId);
}
