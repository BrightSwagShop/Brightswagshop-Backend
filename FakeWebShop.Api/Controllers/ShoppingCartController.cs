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
    private static bool IsValidQuantity(int quantity) => quantity >= 0 && quantity <= 1000000;


    [HttpPost]
    public async Task<ActionResult<ShoppingCartResponse>> Create([FromBody] ShoppingCartRequest request)
    {
        if (request.Items.Any(item => !IsValidQuantity(item.Quantity)))
            return BadRequest("Quantity must be between 0 and 1000000.");

        var createdCart = await service.CreateAsync(request);
        return CreatedAtAction(nameof(GetByUserId), new { userId = createdCart.UserId }, createdCart);
    }

    [HttpPost("{cartId}/apply-discount")]
    public async Task<ActionResult<ShoppingCartResponse>> ApplyDiscount(string cartId, [FromBody] ApplyDiscountRequest request)
    {
        try
        {
            var updatedCart = await service.ApplyDiscountCodeAsync(cartId, request.Code);
            if (updatedCart is null)
                return NotFound();
            return Ok(updatedCart);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
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

    // hele cart verwijderen
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await service.DeleteAsync(id);

        if (!deleted)
            return NotFound();

        return NoContent();
    }

    // 1 item verwijderen
    [HttpDelete("user/{userId}/item")]
    public async Task<IActionResult> RemoveItem(string userId, [FromBody] CartItemRequest request)
    {
        var result = await service.RemoveItemAsync(userId, request);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // 1 item updaten
    [HttpPut("user/{userId}/quantity")]
    public async Task<IActionResult> UpdateQuantity(string userId, [FromBody] CartItemRequest request)
    {
        var result = await service.UpdateQuantityAsync(userId, request);

        if (result is null)
            return NotFound();

        return Ok(result);
    }

    // Cart item toevoegen
    [HttpPost("user/{userId}/items")]
    public async Task<IActionResult> AddItem(string userId, [FromBody] CartItemRequest request)
    {
        if (!IsValidQuantity(request.Quantity))
            return BadRequest("Quantity must be between 0 and 1000000.");

        var result = await service.AddItemAsync(userId, request);
        return Ok(result);
    }
}