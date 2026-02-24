using FakeWebShop.Contracts;
using FakeWebShop.Contracts.Enums;
using FakeWebShop.Domain.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FakeWebShop.Api
{
    [Route("api/producten")]
    [ApiController]
    public class ProductController(IProductenService service, IProductService productService) : ControllerBase
    {
        [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductResponseContract>>> GetAllProducts()
    {
        var items = await productService.GetAllProductsAsync();
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponseContract>> GetProductById([FromRoute] Guid id)
    {
        var item = await productService.GetProductAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] Guid id)
    {
        await productService.DeleteProductAsync(id);
        return NoContent();
    }

    // ---- Drinkfles ----

    [HttpPost("drinkflessen")]
    public async Task<ActionResult<DrinkflesResponseContract>> CreateDrinkfles([FromBody] DrinkflesRequestContract contract)
    {
        var created = await service.CreateDrinkflesAsync(contract);
        return CreatedAtAction(nameof(GetDrinkflesById), new { id = created.Id }, created);
    }

    [HttpGet("drinkflessen")]
    public async Task<ActionResult<IEnumerable<DrinkflesResponseContract>>> GetAllDrinkflessen()
    {
        var items = await service.GetAllDrinkflessenAsync();
        return Ok(items);
    }

    [HttpGet("drinkflessen/{id:guid}")]
    public async Task<ActionResult<DrinkflesResponseContract>> GetDrinkflesById([FromRoute] Guid id)
    {
        var item = await service.GetDrinkflesAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPut("drinkflessen/{id:guid}")]
    public async Task<ActionResult<DrinkflesResponseContract>> UpdateDrinkfles([FromRoute] Guid id, [FromBody] DrinkflesRequestContract contract)
    {
        var updated = await service.UpdateDrinkflesAsync(id, contract);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("drinkflessen/{id:guid}")]
    public async Task<IActionResult> DeleteDrinkfles([FromRoute] Guid id)
    {
        await service.DeleteDrinkflesAsync(id);
        return NoContent();
    }

    // ---- Hoodie ----

    [HttpPost("hoodies")]
    public async Task<ActionResult<HoodieResponseContract>> CreateHoodie([FromBody] HoodieRequestContract contract)
    {
        var created = await service.CreateHoodieAsync(contract);
        return CreatedAtAction(nameof(GetHoodieById), new { id = created.Id }, created);
    }

    [HttpGet("hoodies")]
    public async Task<ActionResult<IEnumerable<HoodieResponseContract>>> GetAllHoodies()
    {
        var items = await service.GetAllHoodiesAsync();
        return Ok(items);
    }

    [HttpGet("hoodies/{id:guid}")]
    public async Task<ActionResult<HoodieResponseContract>> GetHoodieById([FromRoute] Guid id)
    {
        var item = await service.GetHoodieAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPut("hoodies/{id:guid}")]
    public async Task<ActionResult<HoodieResponseContract>> UpdateHoodie([FromRoute] Guid id, [FromBody] HoodieRequestContract contract)
    {
        var updated = await service.UpdateHoodieAsync(id, contract);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("hoodies/{id:guid}")]
    public async Task<IActionResult> DeleteHoodie([FromRoute] Guid id)
    {
        await service.DeleteHoodieAsync(id);
        return NoContent();
    }
        
    }
}
