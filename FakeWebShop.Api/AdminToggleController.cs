using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FakeWebShop.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminToggleController(IBugService _bugService) : ControllerBase
    {
        [HttpPost("toggle-payment-failure")]
        public IActionResult TogglePaymentFailure(bool enabled)
        {
              _bugService.SetPaymentFailure(enabled);
              return Ok();
            
        }
    }
}
