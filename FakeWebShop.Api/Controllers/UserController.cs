using FakeWebShop.Contracts.UserContracts;
using FakeWebShop.Domain.Services.MongoUserServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FakeWebShop.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController(MongoUserService service) : ControllerBase
    {
        [HttpPost("register")]
    public async Task<ActionResult<UserResponseContract>> Register([FromBody] UserRequestContract request)
    {
        var user = await service.Register(request);
        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserResponseContract>> Login([FromBody] UserRequestContract request)
    {
         var user = await service.Login(request);
        if (user == null)
            return Unauthorized();

        return Ok(user);
    }

    
        
    }
}
