using System;
using FakeWebShop.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FakeWebShop.Api.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoryController : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll()
    {
        var categories = Enum.GetValues<CategoryEnum>()
            .Select(c => new
            {
                id = (int)c,
                name = c.ToString()
            });

        return Ok(categories);
    }
}
