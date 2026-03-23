using System;
using FakeWebShop.Domain.Enums;
using FakeWebShop.Persistence.Entities.Discount;


namespace FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;

public interface IDiscountRepository
{
    Task<Discount> CreateAsync(Discount discount);

    Task<Discount?> GetByIdAsync(string id);

    Task<List<Discount>> GetAllAsync();

    Task UpdateAsync(Discount discount);

    Task DeleteAsync(string id);
}