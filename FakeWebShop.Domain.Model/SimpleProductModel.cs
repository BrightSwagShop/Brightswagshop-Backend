using FakeWebShop.Domain.Model.VariantModel;

namespace FakeWebShop.Domain.Model;

public class SimpleProductModel : ProductModel
{
    public List<ColorVariantModel> Kleuren { get; set; } = new();
}
