using System.Security.Claims;
using FakeWebShop.Contracts.Request.UserRequest;
using FakeWebShop.Contracts.Response.UserResponse;
using FakeWebShop.Domain.Services;
using FakeWebShop.Domain.Services.MongoUserServices.MongoInterfaces;
using FakeWebShop.Persistence.PublicUserRepo_s.MongoInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FakeWebShop.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UserController(
    IMongoUserInterface service,
    JwtService jwtService,
    IMongoUserRepository userRepo) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserResponseContract>> Register([FromBody] UserAuthRequestContract request)
    {
        var user = await service.Register(request);
        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserAuthRequestContract request)
    {
        var user = await service.Login(request);

        if (user == null)
            return Unauthorized();

        var userEntity = await userRepo.GetByUsernameAsync(request.Username);
        if (userEntity == null)
            return Unauthorized();

        var token = jwtService.GenerateJwtToken(userEntity);

        return Ok(new
        {
            user,
            token
        });
    }

    [Authorize(AuthenticationSchemes = "CustomJwt", Roles = "User")]
    [HttpPost("favoriteToevoegen")]
    public async Task<ActionResult<UserResponseContract>> VoegFavoriteToe([FromBody] FavoriteRequestContract request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var updatedUser = await service.VoegFavoriteByUserAsync(userId, request);
        return Ok(updatedUser);
    }

    [Authorize(AuthenticationSchemes = "CustomJwt", Roles = "User")]
    [HttpPost("favoriteVerwijderen")]
    public async Task<ActionResult<UserResponseContract>> VerwijderFavorite([FromBody] FavoriteRequestContract request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var updatedUser = await service.RemoveFavoriteAsync(userId, request);
        return Ok(updatedUser);
    }

    [Authorize(AuthenticationSchemes = "CustomJwt", Roles = "User")]
    [HttpGet("me")]
    public async Task<ActionResult<UserResponseContract>> GetMe()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var user = await service.GetByIdAsync(userId);
        return Ok(user);
    }
}