using System;
using FakeWebShop.Persistence.Entities.Model;

namespace FakeWebShop.Persistence.repos;

public interface IProductenRepo
{
    //Drinkfles crud
    Task<Drinkfles> SaveDrinkflesAsync(Drinkfles drinkfles);
    Task<Drinkfles?> GetDrinkflesAsync(Guid id);
    Task<List<Drinkfles>> GetAllDrinkflesAsync();
    Task<Drinkfles> UpdateDrinkflesAsync(Drinkfles drinkfles);
    Task DeleteDrinkflesAsync(Guid id);
    //Mok crud 

     Task<Mok> SaveMokAsync(Drinkfles drinkfles);
    Task<Mok?> GetMokAsync(Guid id);
    Task<List<Mok>> GetAllMokAsync();
    Task<Mok> UpdateMokAsync(Mok mok);
    Task DeleteMokAsync(Guid id);

    //Hoodie

    Task<Hoodie> SaveHoodieAsync(Hoodie hoodie);
    Task<Hoodie?> GetHoodieAsync(Guid id);
    Task<List<Hoodie>> GetAllHoodieAsync();
    Task<Hoodie> UpdateHoodieAsync(Hoodie hoodie);
    Task DeleteHoodieAsync(Guid id);

    //Tshirt

     Task<Tshirt> SaveTshirtAsync(Tshirt tshirt);
    Task<Tshirt?> GetTshirtAsync(Guid id);
    Task<List<Tshirt>> GetAllTshirtAsync();
    Task<Tshirt> UpdateTshirtAsync(Tshirt tshirt);
    Task DeleteTshirtAsync(Guid id);

    //Notebook

    
    Task<NoteBook> SaveNoteBookAsync(NoteBook notebook);
    Task<NoteBook?> GetNotebookAsync( Guid id);
    Task<List<NoteBook>> GetNotebookAsync();
    Task<NoteBook> UpdateNotebookAsync(NoteBook notebook);
    Task DeleteNotebookAsync(Guid id);


    //Pen

    Task<Pen> SavePenAsync(Pen pen);
    Task<Pen?> GetPenAsync( Guid id);
    Task<List<Pen>> GetPenAsync();
    Task<Pen> UpdatePenAsync(Pen pen);
    Task DeletePenAsync(Guid id);

    //Powerbank

    Task<Powerbank> SavePowerbankAsync(Powerbank powerbank);
    Task<Powerbank?> GetPowerbankAsync(Guid id);
    Task<List<Powerbank>> GetPowerbankAsync();
    Task<Powerbank> UpdatePowerbankAsync(Pen pen);
    Task DeletePowerbankAsync(Guid id);

    //Sticker
    Task<Sticker> SaveStickerAsync(Sticker sticker);
    Task<Sticker?> GetStickerAsync(Guid id);
    Task<List<Sticker>> GetStickerAsync();
    Task<Sticker> UpdateStickerAsync(Sticker sticker);
    Task DeleteStickerAsync(Guid id);

    //Totebag
    Task<ToteBag> SaveToteBagAsync(ToteBag totebag);
    Task<ToteBag?> GetTotebagAsync(Guid id);
    Task<List<ToteBag>> GetTotebagAsync();
    Task<ToteBag> UpdateTotebagAsync(ToteBag totebag);
    Task DeleteTotebagAsync(Guid id);




}
