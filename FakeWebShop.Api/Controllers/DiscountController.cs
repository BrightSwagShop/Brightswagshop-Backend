using FakeWebShop.Contracts.Request.DiscountRequest;
using FakeWebShop.Contracts.Response.DiscountResponse;
using FakeWebShop.Domain.Services.Interface_s;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FakeWebShop.Api.Controllers
{
    [ApiController]
    [Route("api/discounts")]
    public class DiscountController(IDiscountService service) : ControllerBase
    {
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<DiscountResponse>> CreateDiscount([FromBody] DiscountRequest discount)
        {
            var createdDiscount = await service.CreateAsync(discount);
            return CreatedAtAction(nameof(GetDiscountById), new { id = createdDiscount.Id }, createdDiscount);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DiscountResponse>> GetDiscountById(string id)
        {
            var discount = await service.GetByIdAsync(id);

            if (discount is null)
                return NotFound();

            return Ok(discount);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DiscountResponse>>> GetAllDiscounts()
        {
            var discounts = await service.GetAllAsync();
            return Ok(discounts);
        }           

        [HttpPut("{id}")]
        public async Task<ActionResult<DiscountResponse>> UpdateDiscount(string id, [FromBody] DiscountRequest discount)
        {
            var updatedDiscount = await service.UpdateAsync(id, discount);
            if (updatedDiscount is null)
                return NotFound();

            return Ok(updatedDiscount);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiscount(string id)
        {
            var deleted = await service.DeleteAsync(id);
            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}