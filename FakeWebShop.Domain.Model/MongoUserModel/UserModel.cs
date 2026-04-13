using System;

namespace FakeWebShop.Domain.Model.MongoUserModel;

public class UserModel
{
    public string Id { get; set; } = null!;
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;

    public List<string> Favorites { get; set; } = new();
    //public List<string> Cart { get; set; } = new();

}
