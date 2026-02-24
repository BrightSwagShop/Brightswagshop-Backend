using System;

namespace FakeWebShop.Domain.Model;

public class DrinkflesModel: DrinkProductModel
{
     public bool IsThermisch { get; set; }

    public DrinkFlesMateriaal Materiaal { get; set; }

    public new bool IsValid()
    {
        return base.IsValid()
               && Inhoud > 0;
    }

}
