using System;
using FakeWebShop.Persistence.Constants;
using FakeWebShop.Persistence.Entities.Discount;
using FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;
using FakeWebShop.Persistence.MongoRepo_s.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace FakeWebShop.Persistence.MongoRepo_s;

public class DiscountRepository : IDiscountRepository
{
    public Task<Discount> CreateAsync(Discount discount)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Discount>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Discount?> GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Discount discount)
    {
        throw new NotImplementedException();
    }


}
