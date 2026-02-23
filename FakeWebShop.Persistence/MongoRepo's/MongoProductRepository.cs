using FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;
using FakeWebShop.Persistence.MongoRepo_s.Options;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using FakeWebShop.Persistence.Entities.BaseProduct;

namespace FakeWebShop.Persistence.MongoRepo_s;

public class MongoProductRepository : IMongoProductRepository
{
    private readonly IMongoCollection<Product> _products;
    public MongoProductRepository(IMongoClient client, IOptions<MongoOptions> options)
    {
        var database = client.GetDatabase(options.Value.Database); // Get Databse
        _products = database.GetCollection<Product>("Products");    // Get Juiste Collection

    }

    // Nieuw product aanmaken
    public async Task CreateAsync(Product product)
    {
        await _products.InsertOneAsync(product);
    }

    // Product delete
    public async Task<bool> DeleteAsync(string id)
    {
        var deleteResult = await _products.DeleteOneAsync(p => p.Id == id);
        return deleteResult.DeletedCount > 0;
    }

    // 1 Product terug geven.
    public async Task<List<Product>> GetAllAsync()
    {
        return await _products.Find(_ => true).ToListAsync();
    }

    // Lijst van product terug geven. 
    public async Task<Product?> GetByIdAsync(string id)
    {
        return await _products
            .Find(Product => Product.Id == id)
            .FirstOrDefaultAsync();
    }




    // Product updaten


    // Alleen updaten wat effectief meegegeven is (Code moet nog herbekeken worden/ Misschien andere manier van te schrijven)
}
