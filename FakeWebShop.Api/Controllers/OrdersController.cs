using FakeWebShop.Contracts.Request.OrderRequest;
using FakeWebShop.Contracts.Response.OrderResponse;
using FakeWebShop.Domain.Services.Interface_s;
using Microsoft.AspNetCore.Mvc;

namespace FakeWebShop.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrdersController(IOrderService service) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<OrderResponse>> Create([FromBody] OrderRequest request)
    {
        var createdOrder = await service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, createdOrder);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderResponse>> GetById(string id)
    {
        var order = await service.GetByIdAsync(id);

        if (order is null)
            return NotFound();

        return Ok(order);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<List<OrderResponse>>> GetByUserId(string userId)
    {
        var orders = await service.GetByUserIdAsync(userId);
        return Ok(orders);
    }

    // Geen Put endpoints
}