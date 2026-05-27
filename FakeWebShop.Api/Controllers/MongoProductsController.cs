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

    // Alle Products by Id 
    [HttpGet("{id}")]
    public async Task<ActionResult<MongoProductResponse>> GetById(string id)
    {
        var product = await service.GetProductById(id);
        if (product is null)
            return NotFound();

        return Ok(product);
    }

    [HttpPost]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<MongoProductResponse>> Create([FromBody] MongoProductRequest request)
    {
        try
        {
            var created = await service.CreateProduct(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminOnly")]
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
            "sportkledij" => ProductTypeEnum.Sportkledij,
            "sokken" => ProductTypeEnum.Sokken,
            "drinkfles" => ProductTypeEnum.Drinkfles,
            "drinkflessen" => ProductTypeEnum.Drinkfles,
            "mok" => ProductTypeEnum.Mok,
            "mokken" => ProductTypeEnum.Mok,
            "onderlegger" => ProductTypeEnum.Onderlegger,
            "onderleggers" => ProductTypeEnum.Onderlegger,
            "balpen" => ProductTypeEnum.Balpen,
            "balpennen" => ProductTypeEnum.Balpen,
            "eendje" => ProductTypeEnum.Eendje,
            "eendjes" => ProductTypeEnum.Eendje,
            _ => null
        };

        if (productType is null)
            return NotFound($"Producttype '{slug}' bestaat niet.");

        var products = await service.GetProductsByTypeAsync(productType.Value);
        return Ok(products);
    }

    // Voor lijst van product voor Favorites
    [HttpPost("by-ids")]
    public async Task<ActionResult<List<MongoProductResponse>>> GetByIds([FromBody] List<string> ids)
    {
        if (ids == null || ids.Count == 0)
            return BadRequest("Geen ids meegegeven.");

        var products = await service.GetProductsByIdsAsync(ids);
        return Ok(products);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<MongoProductResponse>> Update(string id, [FromBody] MongoProductRequest request)
    {
        var updated = await service.UpdateProduct(id, request);

        if (updated is null)
            return NotFound();

        return Ok(updated);
    }
}
