
using Microsoft.AspNetCore.Mvc;

namespace FakeWebShop.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminToggleController : ControllerBase
    {
        [HttpPost("toggle-payment-failure")]
        public IActionResult TogglePaymentFailure(bool enabled)
        {
            // _bugService.SetPaymentFailure(enabled);
            return Ok();

        }
    }
}
