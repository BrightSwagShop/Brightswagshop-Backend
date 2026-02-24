using System;
using FakeWebShop.Domain.Model;


namespace FakeWebShop.Persistence.Entities.Model;

public class Tshirt: ClothingProduct
{
    public PrintType PrintType { get; set; }
    public Tshirt(Guid id, string naam, string beschrijving, decimal prijs,
        string imageUrl, Guid categoryId,
        Maat maat, Stof stof, string kleur,
        PrintType printType)
        : base(id, naam, beschrijving, prijs, imageUrl, categoryId, maat, stof, kleur)
    {
        PrintType = printType;
    }

}
