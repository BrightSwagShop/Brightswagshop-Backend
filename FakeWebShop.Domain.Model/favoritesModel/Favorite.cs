using System;

namespace FakeWebShop.Domain.Model.favoritesModel;

public class Favorite
{
    public string Id { get; set; }
    public string UserId { get; set; }
    public string ProductId { get; set; }
    public DateTime CreatedAt { get; set; }

}
