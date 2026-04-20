using FakeWebShop.Contracts.Request.Products.BaseProductRequest;
using FakeWebShop.Contracts.Response.Products.BaseProductResponse;
using FakeWebShop.Domain.Enums;
namespace FakeWebShop.Domain.Services.MongoInterfaces;

public interface IMongoProductService
{
    Task<MongoProductResponse> CreateProduct(MongoProductRequest product);
    Task<List<MongoProductResponse>> GetProducts();
    Task<MongoProductResponse> GetProductById(string id);
    Task<bool> DeleteProduct(string id);
    // Update Product voor later "Lange code";
    Task<List<MongoProductResponse>> GetProductsByTypeAsync(ProductTypeEnum productType);
    Task<List<MongoProductResponse>> GetProductsByIdsAsync(List<string> ids);
}
