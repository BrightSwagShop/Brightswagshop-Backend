using System;
using FakeWebShop.Persistence.Entities.Model.Enums;

namespace FakeWebShop.Persistence.Entities.Model;

public class Hoodie : ClothingProduct
{
    public bool HasZipper { get;   set; }
    public PocketType PocketType { get;   set; }

    public Hoodie(Guid id, string naam, string beschrijving, decimal prijs,
        string imageUrl, Guid categoryId,
        Maat maat, Stof stof, string kleur,
        bool hasZipper,
        PocketType pocketType)
        : base(id, naam, beschrijving, prijs, imageUrl, categoryId, maat, stof, kleur)
    {
        HasZipper = hasZipper;
        PocketType = pocketType;
    }

}
