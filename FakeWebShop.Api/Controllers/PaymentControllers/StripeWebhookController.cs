using System;
using FakeWebShop.Domain.Services.Interface_s;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace FakeWebShop.Api.Controllers.PaymentControllers;

[ApiController]
[Route("api/webhooks/stripe")]
public class StripeWebhookController(IConfiguration config, IStripeWebhookService stripewebhook) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> HandleWebhook()
    {
        string json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
        string? webhookSecret = config["Stripe:WebhookSecret"];
        string signature = Request.Headers["Stripe-Signature"].ToString();

        if (string.IsNullOrWhiteSpace(webhookSecret))
            return BadRequest("Missing webhook secret");

        if (string.IsNullOrWhiteSpace(signature))
            return BadRequest("Missing Stripe signature");

        try
        {
            Event stripeEvent = EventUtility.ConstructEvent(
                json,
                signature,
                webhookSecret,
                throwOnApiVersionMismatch: false
            );

            await stripewebhook.HandleAsync(stripeEvent);

            return Ok();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Webhook fout: {ex.Message}");
            return BadRequest();
        }
    }
}
