using FakeWebShop.Contracts.Request;
using FakeWebShop.Domain.Services.MongoInterface_s;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FakeWebShop.Api.Controllers
{
    
    [ApiController]
    [Route("api/favorites")]
    public class FavoriteController(IFavoriteService _service) : ControllerBase
    {
        [HttpPost]
    public async Task<IActionResult> Add(FavoriteRequestContract request)
    {
        var userId = User.FindFirst("sub")?.Value 
                    ?? User.Identity?.Name;

        if (userId == null)
            return Unauthorized();

        await _service.AddFavorite(userId, request.ProductId);
        return Ok();
    }
    [HttpDelete]
    public async Task<IActionResult> Remove([FromQuery] string userId, [FromQuery] string productId)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(productId))
            return BadRequest("Missing parameters");

        await _service.RemoveFavorite(userId, productId);

        return Ok();
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> Get(string userId)
    {
        if (string.IsNullOrEmpty(userId))
            return BadRequest("Invalid userId");

        var favorites = await _service.GetFavorites(userId);

        return Ok(favorites);
    }
    }
}
