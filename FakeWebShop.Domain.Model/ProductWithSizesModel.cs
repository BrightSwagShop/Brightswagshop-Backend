using FakeWebShop.Domain.Model.VariantModel;

namespace FakeWebShop.Domain.Model;

public class ProductWithSizesModel : ProductModel
{
    public List<ColorVariantClothesModel> Kleuren { get; set; } = new();

}
