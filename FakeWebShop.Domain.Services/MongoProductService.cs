using System;
using FakeWebShop.Contracts.Request.Products;
using FakeWebShop.Contracts.Request.Products.BaseProductRequest;
using FakeWebShop.Contracts.Response.Products.BaseProductResponse;
using FakeWebShop.Domain.Services.MongoInterfaces;
using FakeWebShop.Domain.Services.MongoServicesMapping;
using FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;

namespace FakeWebShop.Domain.Services;

public class MongoProductService(IMongoProductRepository repo) : IMongoProductService
{
    public async Task<MongoProductResponse> CreateProduct(MongoProductRequest product)
    {
        var productModel = product.ToModel();
        var productEntity = productModel.ToEntity();

        var createdProduct = await repo.CreateAsync(productEntity);
        return createdProduct.ToModel().ToResponse();
    }

    public async Task<bool> DeleteProduct(string id)
    {
        return await repo.DeleteAsync(id);
    }

    public async Task<MongoProductResponse?> GetProductById(string id)
    {
        var product = await repo.GetByIdAsync(id);
        return product?.ToModel().ToResponse();
    }

    public async Task<List<MongoProductResponse>> GetProducts()
    {
        var product = await repo.GetAllAsync();
        return product.Select(p => p.ToModel().ToResponse()).ToList();
    }

    // Update Later aanmaken 

}
