using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FakeWebShop.Persistence.Entities.PublicUser;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;

namespace FakeWebShop.Domain.Services;

public class JwtService
{
    public string GenerateJwtToken(User user)
{
    var key = new SymmetricSecurityKey(
        //secret key nog aanpassen later
        Encoding.UTF8.GetBytes("YOUR_SECRET_KEY_HIER_MIN_16_CHARS")
    );

    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    var userId = ObjectId.Parse(user.Id).ToString();

    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, userId),
        new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
    };

    var token = new JwtSecurityToken(
        issuer: "yourapp",
        audience: "yourapp",
        claims: claims,
        expires: DateTime.UtcNow.AddHours(2),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

}
