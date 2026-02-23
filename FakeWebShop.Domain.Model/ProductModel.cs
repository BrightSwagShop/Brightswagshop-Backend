using System;

namespace FakeWebShop.Domain.Model;

public class ProductModel
{
    public Guid? Id { get; set; }

    public string Naam { get; set; } = string.Empty;

    public string Beschrijving { get; set; } = string.Empty;

    public decimal Prijs { get; set; }

    public string ImageUrl { get; set; } = string.Empty;

    public Guid CategoryId { get; set; }

    public bool IsValid()
    {
        return !string.IsNullOrWhiteSpace(Naam)
               && Prijs > 0
               && CategoryId != Guid.Empty;
    }

}
