using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FakeWebShop.Api.Controllers;


[ApiController]
[Route("api/admins")]
public class AdminController : ControllerBase
{
    [Authorize(AuthenticationSchemes = "AzureAd", Roles = "App.Admin")]
    [HttpGet("admin-only")]
    public IActionResult Get()
    {
        return Ok("Admin works");
    }
}
