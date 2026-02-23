using System;

namespace FakeWebShop.Domain.Model;

public class ClothingProductModel : ProductModel
{
      public Maat Maat { get; set; }
    public Stof Stof { get; set; }
    public string Kleur { get; set; } = string.Empty;

    public new bool IsValid()
    {
        return base.IsValid()
               && !string.IsNullOrWhiteSpace(Kleur);
    }



}
