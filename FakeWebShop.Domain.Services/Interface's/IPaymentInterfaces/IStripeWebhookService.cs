using System;
using Stripe;

namespace FakeWebShop.Domain.Services.Interface_s;

public interface IStripeWebhookService
{
    Task HandleAsync(Event stripeEvent);
}