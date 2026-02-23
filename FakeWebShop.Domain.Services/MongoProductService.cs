using System;
using FakeWebShop.Contracts.Request.Products;
using FakeWebShop.Contracts.Request.Products.BaseProductRequest;
using FakeWebShop.Contracts.Response.Products.BaseProductResponse;
using FakeWebShop.Domain.Services.MongoInterfaces;
using FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;

namespace FakeWebShop.Domain.Services;

public class MongoProductService(IMongoProductRepository repo) : IMongoProductService
{
    public Task<MongoProductResponse> CreateProduct(MongoProductRequest product)
    {
        throw new NotImplementedException();
    }

    public Task DeleteProduct(string id)
    {
        throw new NotImplementedException();
    }

    public Task<MongoProductResponse> GetProductById(string id)
    {
        throw new NotImplementedException();
    }

    public Task<List<MongoProductResponse>> GetProducts()
    {
        throw new NotImplementedException();
    }

    // Later 
}
