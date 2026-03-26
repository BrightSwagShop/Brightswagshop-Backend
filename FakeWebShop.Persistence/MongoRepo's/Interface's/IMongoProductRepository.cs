using System;
using FakeWebShop.Domain.Enums;
using FakeWebShop.Persistence.Entities.BaseProduct;


namespace FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;

public interface IMongoProductRepository
{
    Task<Product> CreateAsync(Product product);
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(string id);
    Task<bool> DeleteAsync(string id);
    Task<List<Product>> GetByTypeAsync(ProductTypeEnum productType);

    // Volledige vervanging
    // Task<bool> UpdateAsync(Product product);



}
