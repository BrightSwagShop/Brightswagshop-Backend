using System;
using FakeWebShop.Contracts.Request.Products.BaseProductRequest;
using FakeWebShop.Contracts.Response.Products.BaseProductResponse;
using FakeWebShop.Domain.Services.MongoInterfaces;
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
    public async Task<ActionResult<MongoProductResponse>> Create([FromBody] MongoProductRequest request)
    {
        var created = await service.CreateProduct(request);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await service.DeleteProduct(id);

        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
