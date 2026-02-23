namespace FakeWebShop.Domain.Model.VariantModel;

public class SizeVariantModel
{
    public required string Maat { get; set; }
    public int Stock { get; set; }
    public required string Sku { get; set; } // Stock Keeping unit
}
