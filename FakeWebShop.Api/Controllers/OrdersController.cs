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

    [HttpPost("from-cart/{userId}")]
    public async Task<ActionResult<OrderResponse>> CreateFromCart(string userId)
    {
        try
        {
            var createdOrder = await service.CreateFromCartAsync(userId);
            return CreatedAtAction(nameof(GetById), new { id = createdOrder.Id }, createdOrder);
        }
        catch (Exception ex)
        {
            var msg = ex.Message ?? string.Empty;
            if (msg.Contains("not found", StringComparison.OrdinalIgnoreCase))
                return NotFound(msg);
            if (msg.Contains("empty", StringComparison.OrdinalIgnoreCase))
                return BadRequest(msg);
            return StatusCode(500, msg);
        }
    }
}