using System;

namespace FakeWebShop.Persistence.Entities.Model;

public class NoteBook: Product
{
    public NoteBook(Guid id,
        string naam,
        string beschrijving,
        decimal prijs,
        string imageUrl,
        Guid categoryId,
        string formaat,
        string kleur): base(id, naam, beschrijving, prijs, imageUrl, categoryId)
    {
        Formaat = formaat;
        Kleur = kleur;
    }

    public string Formaat{ get; set; }
    public string Kleur{ get; set; }

}
