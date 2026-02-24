using System;
using FakeWebShop.Contracts;
using FakeWebShop.Contracts.Enums;

namespace FakeWebShop.Domain.Services.Interfaces;

public interface IProductenService
{
     // Drinkfles
    Task<DrinkflesResponseContract> CreateDrinkflesAsync(DrinkflesRequestContract contract);
    Task<DrinkflesResponseContract?> GetDrinkflesAsync(Guid id);
    Task<IEnumerable<DrinkflesResponseContract>> GetAllDrinkflessenAsync();
    Task<DrinkflesResponseContract?> UpdateDrinkflesAsync(Guid id, DrinkflesRequestContract contract);
    Task DeleteDrinkflesAsync(Guid id);

    // Hoodie
    Task<HoodieResponseContract> CreateHoodieAsync(HoodieRequestContract contract);
    Task<HoodieResponseContract?> GetHoodieAsync(Guid id);
    Task<IEnumerable<HoodieResponseContract>> GetAllHoodiesAsync();
    Task<HoodieResponseContract?> UpdateHoodieAsync(Guid id, HoodieRequestContract contract);
    Task DeleteHoodieAsync(Guid id);

}
