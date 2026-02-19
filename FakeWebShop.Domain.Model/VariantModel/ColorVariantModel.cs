namespace FakeWebShop.Domain.Model.VariantModel;

public class ColorVariantModel
{
    public required string Kleur { get; set; }
    public required string ImageUrl { get; set; }
    public int Stock { get; set; }
    public required string Sku { get; set; } // Stock Keeping unit
}
