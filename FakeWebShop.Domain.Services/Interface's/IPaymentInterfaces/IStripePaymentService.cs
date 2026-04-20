using System;
using FakeWebShop.Contracts.Response.OrderResponse;
using FakeWebShop.Domain.Model.Order;

namespace FakeWebShop.Domain.Services.Interface_s;

public interface IStripePaymentService
{
    Task<CheckoutSessionResult> CreateCheckoutSessionAsync(OrderResponse order);
}
