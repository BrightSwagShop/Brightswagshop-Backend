namespace FakeWebShop.Domain.Model.VariantModel;

public class ColorVariantClothesModel
{
    public required string Kleur { get; set; }
    public required string ImageUrl { get; set; }
    public List<SizeVariantModel> Maten { get; set; } = new();
}
