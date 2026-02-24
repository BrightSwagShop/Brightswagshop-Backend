using System;

namespace FakeWebShop.Domain.Model;

public class DrinkProductModel : ProductModel
{
     public int Inhoud { get; set; }

    public string Kleur { get; set; } = string.Empty;

    public new bool IsValid()
    {
        return base.IsValid()
               && Inhoud > 0
               && !string.IsNullOrWhiteSpace(Kleur);
    }

}
