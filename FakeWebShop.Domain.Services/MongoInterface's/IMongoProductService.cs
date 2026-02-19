using FakeWebShop.Contracts.Request.Products.BaseProductRequest;
using FakeWebShop.Contracts.Response.Products.BaseProductResponse;
namespace FakeWebShop.Domain.Services.MongoInterfaces;

public interface IMongoProductService
{
    Task<MongoProductResponse> CreateProduct(MongoProductRequest product);
    Task<List<MongoProductResponse>> GetProducts();
    Task<MongoProductResponse> GetProductById(string id);
    Task DeleteProduct(string id);
    // Update Product voor later "Lange code";
}
