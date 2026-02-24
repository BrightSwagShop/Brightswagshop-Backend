using System;

namespace FakeWebShop.Contracts;

public class ProductSummaryResponseContract
{
     public Guid Id { get; set; }
    public string Naam { get; set; } = string.Empty;
    public decimal Prijs { get; set; }
    public string ImageUrl { get; set; } = string.Empty;

}
