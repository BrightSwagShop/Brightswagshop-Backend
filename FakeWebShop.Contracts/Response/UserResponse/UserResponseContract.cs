using System;

namespace FakeWebShop.Contracts.Response.UserResponse;

public class UserResponseContract
{
    public string Id { get; set; } = null!;
    public string Username { get; set; } = null!;
    public List<string> Favorites { get; set; } = new();
}
