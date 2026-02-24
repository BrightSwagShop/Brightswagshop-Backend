using System;

namespace FakeWebShop.Domain.Model;

public class HoodieModel : ClothingProductModel
{
    public bool HasZipper { get; set; }

    public PocketType PocketType { get; set; }

    

}
