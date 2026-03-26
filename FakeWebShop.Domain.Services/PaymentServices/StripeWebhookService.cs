using System;
using FakeWebShop.Domain.Enums;
using FakeWebShop.Domain.Services.Interface_s;
using Stripe;
using Stripe.Checkout;

namespace FakeWebShop.Domain.Services;

public class StripeWebhookService(IOrderService orderService) : IStripeWebhookService
{
    public async Task HandleAsync(Event stripeEvent)
    {
        Console.WriteLine($"Webhook ontvangen: {stripeEvent.Type}");

        switch (stripeEvent.Type)
        {
            case "checkout.session.completed":
                await HandleCheckoutSessionCompletedAsync(stripeEvent);
                break;

            default:
                Console.WriteLine($"Onbehandeld event type: {stripeEvent.Type}");
                break;
        }
    }

    private async Task HandleCheckoutSessionCompletedAsync(Event stripeEvent)
    {
        var session = stripeEvent.Data.Object as Session;

        if (session is null)
            throw new Exception("Stripe session is null.");

        if (!session.Metadata.TryGetValue("OrderId", out string? orderId) || string.IsNullOrWhiteSpace(orderId))
        {
            throw new Exception("OrderId ontbreekt in Stripe metadata");
        }

        await orderService.UpdatePaymentStatusAsync(
            orderId,
            PaymentStatusEnum.Paid,
            session.Id
     );

        Console.WriteLine($"Betaling succesvol afgerond. SessionId: {session.Id}");

        // TODO:
        // order ophalen via session.Id
        // order status op Paid zetten


    }
}
