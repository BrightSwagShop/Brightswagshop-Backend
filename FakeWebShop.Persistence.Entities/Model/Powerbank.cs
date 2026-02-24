using System;
using FakeWebShop.Domain.Model;


namespace FakeWebShop.Persistence.Entities.Model;

public class Powerbank: Product
{
    public Powerbank( Guid id,
        string naam,
        string beschrijving,
        decimal prijs,
        string imageUrl,
        Guid categoryId,
        int capaciteitmAh,
        InputType inputType,
        OutputType outputType,
        bool ondersteuntFastCharging,
        int portTotaal
        ): base(id, naam,beschrijving,prijs,imageUrl,categoryId)
    {
        CapaciteitmAh = capaciteitmAh;
        InputType = inputType;
        OutputType = outputType;
       
        OndersteuntFastCharging = ondersteuntFastCharging;
        PortTotaal = portTotaal;
        
    }
    public int CapaciteitmAh{ get; set; }
    public InputType InputType { get; set; }
    public OutputType OutputType { get; set; }

    public bool OndersteuntFastCharging{ get; set; }
    public int PortTotaal{get;set;}

}
