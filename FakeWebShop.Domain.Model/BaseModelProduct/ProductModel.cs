using FakeWebShop.Domain.Enums;

namespace FakeWebShop.Domain.Models;

public abstract class ProductModel
{
    public string Id { get; set; } = null!;
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public CategoryEnum Category { get; set; }
    public ProductTypeEnum ProductType { get; set; }
    public bool IsActive { get; set; }
}

