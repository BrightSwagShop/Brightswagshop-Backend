using System;

namespace FakeWebShop.Contracts.Request.UserRequest;

public class UserAuthRequestContract
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}
