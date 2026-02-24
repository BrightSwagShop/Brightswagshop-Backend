using System;
using System.Xml.Schema;
using FakeWebShop.Domain.Model;
 

namespace FakeWebShop.Persistence.Entities.Model;

public abstract class ClothingProduct: Product
{
    protected ClothingProduct(Guid id, string naam,string beschrijving, decimal prijs, string imageUrl, Guid categoryId, Maat maat, Stof stof,string kleur)
    : base(id, naam,beschrijving,prijs,imageUrl,categoryId)
    {
        Maat = maat;
        Stof = stof;
        Kleur = kleur;

        
    }
    public Maat Maat{ get; set; }
    public Stof Stof{ get; set; }
    public string Kleur{ get; set; }
    


     

}
