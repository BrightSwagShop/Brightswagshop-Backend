using FakeWebShop.Contracts.Request.ContactRequest;
using FakeWebShop.Domain.Services.Interface_s;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FakeWebShop.Api.Controllers
{
    [ApiController]
    [Route("api/contact")]  
    public class ContactController(IEmailService emailService)  : ControllerBase
    {
          [HttpPost]
    public async Task<IActionResult> SendContactMessage([FromBody] ContactRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName) ||
            string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new { error = "Required fields are missing." });
        }

        await emailService.SendContactMailAsync(request);

        return Ok(new { message = "Contact message sent successfully." });
    }
    }
}
