using System;

namespace FakeWebShop.Contracts;

public class CategoryResponseContract
{
    public Guid Id { get; set; }
    public string Naam { get; set; } = string.Empty;

}
