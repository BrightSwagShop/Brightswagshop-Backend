using System;
using FakeWebShop.Contracts.Request.Products.BaseProductRequest;
using FakeWebShop.Contracts.Response.Products.BaseProductResponse;
using FakeWebShop.Domain.Enums;
using FakeWebShop.Domain.Services.MongoInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FakeWebShop.Api.Controllers;

[ApiController]
[Route("api/products")]
public class MongoProductsController(IMongoProductService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<MongoProductResponse>>> GetAll()
    {
        var products = await service.GetProducts();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<MongoProductResponse>> GetById(string id)
    {
        var product = await service.GetProductById(id);
        if (product is null)
            return NotFound();

        return Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<MongoProductResponse>> Create([FromBody] MongoProductRequest request)
    {
        var created = await service.CreateProduct(request);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await service.DeleteProduct(id);

        if (!deleted)
            return NotFound();
        return NoContent();
    }

    [HttpGet("type/{slug}")]
    public async Task<ActionResult<List<MongoProductResponse>>> GetProductsByType(string slug)
    {
        ProductTypeEnum? productType = slug.ToLower() switch
        {
            "tshirt" => ProductTypeEnum.TShirt,
            "hoodie" => ProductTypeEnum.Hoodie,
            "mok" => ProductTypeEnum.Mok,
            "sticker" => ProductTypeEnum.Sticker,
            _ => null
        };

        if (productType is null)
            return NotFound($"Producttype '{slug}' bestaat niet.");

        var products = await service.GetProductsByTypeAsync(productType.Value);
        return Ok(products);
    }
}
