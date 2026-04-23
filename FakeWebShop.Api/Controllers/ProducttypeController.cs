using System;
using FakeWebShop.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FakeWebShop.Api.Controllers;

[ApiController]
[Route("api/producttypes")]
public class ProductTypeController : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<object>> GetAll()
    {
        var productTypes = Enum.GetValues<ProductTypeEnum>()
            .Select(pt => new
            {
                name = pt.ToString(),
                slug = pt.ToString().ToLower()
            });

        return Ok(productTypes);
    }
}