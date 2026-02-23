using System;
using FakeWebShop.Persistence.Entities.Model;
using Microsoft.EntityFrameworkCore;

namespace FakeWebShop.Persistence.repos;

public class ProductenRepo(ProductDbContext dbContext) : IProductenRepo
{
     // GENERIEK: DELETE
    public async Task DeleteAsync<T>(Guid id) where T : class
    {
        var existing = await dbContext.Set<T>().FindAsync(id);
        if (existing == null) throw new Exception($"{typeof(T).Name} not found");

        dbContext.Set<T>().Remove(existing);
        await dbContext.SaveChangesAsync();
    }

    // GENERIEK: GET ALL
    public Task<List<T>> GetAllAsync<T>() where T : class
        => dbContext.Set<T>().ToListAsync();

    // GENERIEK: GET ONE
    public Task<T?> GetAsync<T>(Guid id) where T : class
        => dbContext.Set<T>().FindAsync(id).AsTask();

    // GENERIEK: SAVE
    public async Task<T> SaveAsync<T>(T entity) where T : class
    {
        dbContext.Set<T>().Add(entity);
        await dbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<T> UpdateAsync<T>(Guid id, T entity) where T : class
    {
        var existing = await dbContext.Set<T>().FindAsync(id);
        if (existing == null) throw new Exception($"{typeof(T).Name} not found");

        // kopieer alle scalar props van entity naar existing
        dbContext.Entry(existing).CurrentValues.SetValues(entity);

        await dbContext.SaveChangesAsync();
        return existing;
    }
    // --- jouw methods worden 1-liners ---

    public Task DeleteDrinkflesAsync(Guid id) => DeleteAsync<Drinkfles>(id);
    public Task DeleteHoodieAsync(Guid id) => DeleteAsync<Hoodie>(id);
    public Task DeleteMokAsync(Guid id) => DeleteAsync<Mok>(id);
    public Task DeleteNotebookAsync(Guid id) => DeleteAsync<NoteBook>(id);
    public Task DeletePenAsync(Guid id) => DeleteAsync<Pen>(id);
    public Task DeletePowerbankAsync(Guid id) => DeleteAsync<Powerbank>(id);
    public Task DeleteStickerAsync(Guid id) => DeleteAsync<Sticker>(id);
    public Task DeleteTotebagAsync(Guid id) => DeleteAsync<ToteBag>(id);
    public Task DeleteTshirtAsync(Guid id) => DeleteAsync<Tshirt>(id);

    public Task<List<Drinkfles>> GetAllDrinkflesAsync() => GetAllAsync<Drinkfles>();
    public Task<List<Hoodie>> GetAllHoodieAsync() => GetAllAsync<Hoodie>();
    public Task<List<Mok>> GetAllMokAsync() => GetAllAsync<Mok>();
    public Task<List<Tshirt>> GetAllTshirtAsync() => GetAllAsync<Tshirt>();

    public Task<Drinkfles?> GetDrinkflesAsync(Guid id) => GetAsync<Drinkfles>(id);
    public Task<Hoodie?> GetHoodieAsync(Guid id) => GetAsync<Hoodie>(id);
    public Task<Mok?> GetMokAsync(Guid id) => GetAsync<Mok>(id);
    public Task<NoteBook?> GetNotebookAsync(Guid id) => GetAsync<NoteBook>(id);
    public Task<Pen?> GetPenAsync(Guid id) => GetAsync<Pen>(id);
    public Task<Powerbank?> GetPowerbankAsync(Guid id) => GetAsync<Powerbank>(id);
    public Task<Sticker?> GetStickerAsync(Guid id) => GetAsync<Sticker>(id);
    public Task<ToteBag?> GetTotebagAsync(Guid id) => GetAsync<ToteBag>(id);
    public Task<Tshirt?> GetTshirtAsync(Guid id) => GetAsync<Tshirt>(id);

    public Task<Drinkfles> SaveDrinkflesAsync(Drinkfles x) => SaveAsync(x);
    public Task<Hoodie> SaveHoodieAsync(Hoodie x) => SaveAsync(x);
    public Task<Mok> SaveMokAsync(Mok x) => SaveAsync(x);
    public Task<NoteBook> SaveNoteBookAsync(NoteBook x) => SaveAsync(x);
    public Task<Pen> SavePenAsync(Pen x) => SaveAsync(x);
    public Task<Powerbank> SavePowerbankAsync(Powerbank x) => SaveAsync(x);
    public Task<Sticker> SaveStickerAsync(Sticker x) => SaveAsync(x);
    public Task<ToteBag> SaveToteBagAsync(ToteBag x) => SaveAsync(x);
    public Task<Tshirt> SaveTshirtAsync(Tshirt x) => SaveAsync(x);

    public Task<Drinkfles> UpdateDrinkflesAsync(Guid id, Drinkfles x) => UpdateAsync(id, x);
    public Task<Hoodie> UpdateHoodieAsync(Guid id, Hoodie x) => UpdateAsync(id, x);
    public Task<Mok> UpdateMokAsync(Guid id, Mok x) => UpdateAsync(id, x);
    public Task<NoteBook> UpdateNotebookAsync(Guid id, NoteBook x) => UpdateAsync(id, x);
    public Task<Pen> UpdatePenAsync(Guid id, Pen x) => UpdateAsync(id, x);
    public Task<Powerbank> UpdatePowerbankAsync(Guid id, Powerbank x) => UpdateAsync(id, x);
    public Task<Sticker> UpdateStickerAsync(Guid id, Sticker x) => UpdateAsync(id, x);
    public Task<ToteBag> UpdateTotebagAsync(Guid id, ToteBag x) => UpdateAsync(id, x);
    public Task<Tshirt> UpdateTshirtAsync(Guid id, Tshirt x) => UpdateAsync(id, x);
}
