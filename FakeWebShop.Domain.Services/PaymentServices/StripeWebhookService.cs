using System;
using FakeWebShop.Domain.Enums;
using FakeWebShop.Domain.Services.Interface_s;
using FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;
using Stripe;
using Stripe.Checkout;

namespace FakeWebShop.Domain.Services;

// stripe listen --forward-to http://localhost:5076/api/webhooks/stripe om de Betaling te kunnen testen
public class StripeWebhookService(
    IOrderService orderService,
    IShoppingCartRepository cartRepo) : IStripeWebhookService
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
            throw new Exception("OrderId ontbreekt in Stripe metadata");

        await orderService.UpdatePaymentStatusAsync(
            orderId,
            PaymentStatusEnum.Paid,
            session.Id
        );

        var order = await orderService.GetByIdAsync(orderId);

        if (order is null)
            throw new Exception("Order not found after successful payment.");

        var cart = await cartRepo.GetByUserIdAsync(order.UserId);

        if (cart is not null)
        {
            await cartRepo.DeleteAsync(cart.Id);
            Console.WriteLine($"Cart verwijderd voor user {order.UserId}");
        }

        Console.WriteLine($"Betaling succesvol afgerond. SessionId: {session.Id}");
    }
}