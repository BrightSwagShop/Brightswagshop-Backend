using FakeWebShop.Domain.Model.VariantModel;
using FakeWebShop.Domain.Models;
namespace FakeWebShop.Domain.Model;

public class MugProductModel : ProductModel
{
    public List<ColorVariantModel> Kleuren { get; set; } = new();
}
