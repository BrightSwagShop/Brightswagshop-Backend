using System;
using FakeWebShop.Contracts;
using FakeWebShop.Contracts.Enums;
using FakeWebShop.Domain.Services.Interfaces;
using FakeWebShop.Domain.Services.Mapping;
using FakeWebShop.Persistence.Entities.Model;
using FakeWebShop.Persistence.Interfaces;

namespace FakeWebShop.Domain.Services.services;

public class ProductenService(IProductenRepo repository) : IProductenService
{
    public async Task<DrinkflesResponseContract> CreateDrinkflesAsync(DrinkflesRequestContract contract)
    {
        var model = contract.AsModel();
        var entity = model.AsEntity();

        var created = await repository.SaveAsync(entity);

        return created.AsModel().AsContract();
    }    
    public async Task<DrinkflesResponseContract?> GetDrinkflesAsync(Guid id)
    {
        var entity = await repository.GetAsync<Drinkfles>(id);
        if (entity == null) return null;

        return entity.AsModel().AsContract();
    }

   public async Task<IEnumerable<DrinkflesResponseContract>> GetAllDrinkflessenAsync()
    {
        var entities = await repository.GetAllAsync<Drinkfles>();
        return entities.Select(e => e.AsModel().AsContract());
    }

   public async Task<DrinkflesResponseContract?> UpdateDrinkflesAsync(Guid id, DrinkflesRequestContract contract)
{
    var model = contract.AsModel();
    model.Id = id;

    try
    {
        var updated = await repository.UpdateAsync(id, model.AsEntity());
        return updated.AsModel().AsContract();
    }
    catch
    {
        return null;
    }
}

   public async Task DeleteDrinkflesAsync(Guid id)
{
    await repository.DeleteAsync<Drinkfles>(id);
}

    // =========================
    // HOODIE
    // =========================

    public async Task<HoodieResponseContract> CreateHoodieAsync(HoodieRequestContract contract)
    {
        var model = contract.AsModel();
        var entity = model.AsEntity();

        var created = await repository.SaveAsync(entity);

        return created.AsModel().AsContract();
    }

    public async Task<HoodieResponseContract?> GetHoodieAsync(Guid id)
    {
        var entity = await repository.GetAsync<Hoodie>(id);
        if (entity == null) return null;

        return entity.AsModel().AsContract();
    }

  public async Task<IEnumerable<HoodieResponseContract>> GetAllHoodiesAsync()
    {
        var entities = await repository.GetAllAsync<Hoodie>();
        return entities.Select(e => e.AsModel().AsContract());
    }

    public async Task<HoodieResponseContract?> UpdateHoodieAsync(Guid id, HoodieRequestContract contract)
    {
        var model = contract.AsModel();
        model.Id = id;

        try
        {
            var updated = await repository.UpdateAsync(id, model.AsEntity());
            return updated.AsModel().AsContract();
        }
        catch
        {
            return null;
        }
    }

    public async Task DeleteHoodieAsync(Guid id)
    {
        await repository.DeleteAsync<Hoodie>(id);
    }

}
