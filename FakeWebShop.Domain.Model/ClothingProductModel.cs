using FakeWebShop.Domain.Model.VariantModel;
using FakeWebShop.Domain.Models;

namespace FakeWebShop.Domain.Model;

public class ClothingProductModel : ProductModel
{
    public List<ColorVariantClothesModel> Kleuren { get; set; } = new();

}
