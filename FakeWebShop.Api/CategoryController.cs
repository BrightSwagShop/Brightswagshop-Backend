using FakeWebShop.Contracts;
using FakeWebShop.Domain.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FakeWebShop.Api
{
     [ApiController]
     [Route("api/categories")]
    public class CategoryController(ICategoryService service)  : ControllerBase
    {
            [HttpPost]
    public async Task<ActionResult<CategoryResponseContract>> Create([FromBody] CategoryRequestContract contract)
    {
        var created = await service.CreateCategoryAsync(contract);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponseContract>>> GetAll()
    {
        var items = await service.GetAllCategoriesAsync();
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryResponseContract>> GetById([FromRoute] Guid id)
    {
        var item = await service.GetCategoryAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CategoryResponseContract>> Update([FromRoute] Guid id, [FromBody] CategoryRequestContract contract)
    {
        var updated = await service.UpdateCategoryAsync(contract, id);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id)
    {
        await service.DeleteCategoryAsync(id);
        return NoContent();
    }

    [HttpGet("{id:guid}/products")]
    public async Task<ActionResult<List<ProductResponseContract>>> GetProducts([FromRoute] Guid id)
    {
        var products = await service.GetAllProductsFromCategoryAsync(id);
        return Ok(products);
    }

    }
}
