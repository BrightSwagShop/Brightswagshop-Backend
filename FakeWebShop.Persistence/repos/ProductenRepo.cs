using System;
using FakeWebShop.Persistence.Entities.Model;

namespace FakeWebShop.Persistence.repos;

public class ProductenRepo(ProductDbContext dbContext) : IProductenRepo
{
    public Task DeleteDrinkflesAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteHoodieAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteMokAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteNotebookAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task DeletePenAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task DeletePowerbankAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteStickerAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTotebagAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task DeleteTshirtAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Drinkfles>> GetAllDrinkflesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<Hoodie>> GetAllHoodieAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<Mok>> GetAllMokAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<Tshirt>> GetAllTshirtAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Drinkfles?> GetDrinkflesAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Hoodie?> GetHoodieAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<Mok?> GetMokAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<NoteBook?> GetNotebookAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<NoteBook>> GetNotebookAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Pen?> GetPenAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Pen>> GetPenAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Powerbank?> GetPowerbankAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Powerbank>> GetPowerbankAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Sticker?> GetStickerAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Sticker>> GetStickerAsync()
    {
        throw new NotImplementedException();
    }

    public Task<ToteBag?> GetTotebagAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<ToteBag>> GetTotebagAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Tshirt?> GetTshirtAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<Drinkfles> SaveDrinkflesAsync(Drinkfles drinkfles)
    {
         dbContext.DrinkFlessen.Add(drinkfles);
        await dbContext.SaveChangesAsync();
        return drinkfles;
    }

    public Task<Hoodie> SaveHoodieAsync(Hoodie hoodie)
    {
        dbContext.DrinkFlessen.Add(drinkfles);
        await dbContext.SaveChangesAsync();
        return drinkfles;
    }

    public Task<Mok> SaveMokAsync(Drinkfles drinkfles)
    {
        throw new NotImplementedException();
    }

    public Task<NoteBook> SaveNoteBookAsync(NoteBook notebook)
    {
        throw new NotImplementedException();
    }

    public Task<Pen> SavePenAsync(Pen pen)
    {
        throw new NotImplementedException();
    }

    public Task<Powerbank> SavePowerbankAsync(Powerbank powerbank)
    {
        throw new NotImplementedException();
    }

    public Task<Sticker> SaveStickerAsync(Sticker sticker)
    {
        throw new NotImplementedException();
    }

    public Task<ToteBag> SaveToteBagAsync(ToteBag totebag)
    {
        throw new NotImplementedException();
    }

    public Task<Tshirt> SaveTshirtAsync(Tshirt tshirt)
    {
        throw new NotImplementedException();
    }

    public Task<Drinkfles> UpdateDrinkflesAsync(Drinkfles drinkfles)
    {
        throw new NotImplementedException();
    }

    public Task<Hoodie> UpdateHoodieAsync(Hoodie hoodie)
    {
        throw new NotImplementedException();
    }

    public Task<Mok> UpdateMokAsync(Mok mok)
    {
        throw new NotImplementedException();
    }

    public Task<NoteBook> UpdateNotebookAsync(NoteBook notebook)
    {
        throw new NotImplementedException();
    }

    public Task<Pen> UpdatePenAsync(Pen pen)
    {
        throw new NotImplementedException();
    }

    public Task<Powerbank> UpdatePowerbankAsync(Pen pen)
    {
        throw new NotImplementedException();
    }

    public Task<Sticker> UpdateStickerAsync(Sticker sticker)
    {
        throw new NotImplementedException();
    }

    public Task<ToteBag> UpdateTotebagAsync(ToteBag totebag)
    {
        throw new NotImplementedException();
    }

    public Task<Tshirt> UpdateTshirtAsync(Tshirt tshirt)
    {
        throw new NotImplementedException();
    }
}
