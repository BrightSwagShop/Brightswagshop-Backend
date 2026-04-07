using System;

namespace FakeWebShop.Contracts.Response;

public class FavoriteResponseContract
{
     public string Id { get; set; }
    public string ProductId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

}
