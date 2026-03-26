using FakeWebShop.Contracts.Response.OrderResponse;
using FakeWebShop.Domain.Services.Interface_s;
using Microsoft.Extensions.Configuration;
using Stripe.Checkout;

namespace FakeWebShop.Domain.Services;

public class StripePaymentService(IConfiguration config) : IStripePaymentService
{
    public async Task<CheckoutSessionResult> CreateCheckoutSessionAsync(OrderResponse order)
    {
        List<SessionLineItemOptions> lineItems = new();

        foreach (OrderItemResponse item in order.Items)
        {
            lineItems.Add(new SessionLineItemOptions
            {
                Quantity = item.Quantity,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "eur",
                    UnitAmount = (long)(item.UnitPrice * 100),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = item.ProductName
                    }
                }
            });
        }

        var localBaseUrl = config["Frontend:LocalBaseUrl"];

        SessionCreateOptions options = new()
        {
            Mode = "payment",
            SuccessUrl = $"{localBaseUrl}/success", // Checkout Session Id nog toevoegen "?session_id={{CHECKOUT_SESSION_ID}}" 
            CancelUrl = $"{localBaseUrl}/cancel",
            LineItems = lineItems,
            Metadata = new Dictionary<string, string>
            {
                { "OrderId", order.Id }
            }
        };

        SessionService sessionService = new();
        Session session = await sessionService.CreateAsync(options);

        return new CheckoutSessionResult
        {
            SessionId = session.Id,
            SessionUrl = session.Url
        };
    }
}
