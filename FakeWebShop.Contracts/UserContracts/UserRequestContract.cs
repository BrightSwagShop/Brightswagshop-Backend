using System;

namespace FakeWebShop.Contracts.UserContracts;

public class UserRequestContract
{
    
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Id { get; set; }= null!;
    public string ProductId { get; set; }= null!;
     

}
