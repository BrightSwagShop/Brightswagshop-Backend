using System;
using FakeWebShop.Domain.Services.Interface_s;
using Microsoft.AspNetCore.Mvc;

namespace FakeWebShop.Api.Controllers.PaymentControllers;

[ApiController]
[Route("api/payments")]
public class StripePaymentsController(IStripePaymentService paymentService, IOrderService orderService) : ControllerBase
{
    [HttpPost("{orderId}/checkout")]
    public async Task<IActionResult> CreateCheckoutSession(string orderId)
    {
        var order = await orderService.GetByIdAsync(orderId);

        if (order is null)
            return NotFound("Order not found.");

        if (order.Items is null || order.Items.Count == 0)
            return BadRequest("Order has no items.");

        var result = await paymentService.CreateCheckoutSessionAsync(order);

        await orderService.SetStripeCheckoutSessionIdAsync(orderId, result.SessionId);

        return Ok(result);
    }
}
