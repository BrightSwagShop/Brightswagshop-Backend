using FakeWebShop.Contracts.Request.ApplyDiscountRequest;
using FakeWebShop.Contracts.Request.CartRequest;
using FakeWebShop.Contracts.Response.CartResponse;
using FakeWebShop.Domain.Services.Interface_s;
using Microsoft.AspNetCore.Mvc;

namespace FakeWebShop.Api.Controllers;

[ApiController]
[Route("api/shoppingcarts")]
public class ShoppingCartController(IShoppingCartService service) : ControllerBase
{


    [HttpPost]
    public async Task<ActionResult<ShoppingCartResponse>> Create([FromBody] ShoppingCartRequest request)
    {
        var createdCart = await service.CreateAsync(request);
        return CreatedAtAction(nameof(GetByUserId), new { userId = createdCart.UserId }, createdCart);
    }

    [HttpPost("{cartId}/apply-discount")]
    public async Task<ActionResult<ShoppingCartResponse>> ApplyDiscount(string cartId, [FromBody] ApplyDiscountRequest request)
    {
        var updatedCart = await service.ApplyDiscountCodeAsync(cartId, request.Code);

        if (updatedCart is null)
            return NotFound();

        return Ok(updatedCart);
    }

    [HttpGet("user/{userId}")]
    public async Task<ActionResult<ShoppingCartResponse>> GetByUserId(string userId)
    {
        var cart = await service.GetByUserIdAsync(userId);

        if (cart is null)
            return NotFound();

        return Ok(cart);
    }

    [HttpGet("session/{sessionId}")]
    public async Task<ActionResult<ShoppingCartResponse>> GetBySessionId(string sessionId)
    {
        var cart = await service.GetBySessionIdAsync(sessionId);

        if (cart is null)
            return NotFound();

        return Ok(cart);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await service.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }
}