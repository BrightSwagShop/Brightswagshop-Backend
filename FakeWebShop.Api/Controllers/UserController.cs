using FakeWebShop.Contracts.UserContracts;
using FakeWebShop.Domain.Services;
using FakeWebShop.Domain.Services.MongoServicesMapping.MongoUserMapping;
using FakeWebShop.Domain.Services.MongoUserServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FakeWebShop.Api.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController(MongoUserService service, JwtService jwtService) : ControllerBase
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
         var userEntity = UserMapping.ToEntity(user);
        if (user == null)
            return Unauthorized();
            var token = jwtService.GenerateJwtToken(userEntity);

            return Ok(new { user, token });

        return Ok(user);
    }

    
    
        
    }
}
