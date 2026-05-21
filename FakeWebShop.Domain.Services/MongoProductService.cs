using FakeWebShop.Contracts.Request.Products.BaseProductRequest;
using FakeWebShop.Contracts.Response;
using FakeWebShop.Contracts.Response.Products.BaseProductResponse;
using FakeWebShop.Domain.Enums;
using FakeWebShop.Domain.Services.Interface_s;
using FakeWebShop.Domain.Services.MongoInterfaces;
using FakeWebShop.Domain.Services.MongoServicesMapping;
using FakeWebShop.Persistence.Entities;
using FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;

namespace FakeWebShop.Domain.Services;

public class MongoProductService(IMongoProductRepository repo, IDebugStateService debugService) : IMongoProductService
{

    public async Task<MongoProductResponse> CreateProduct(MongoProductRequest product)
    {
        var productModel = product.ToModel();
        var productEntity = productModel.ToEntity();

        var createdProduct = await repo.CreateAsync(productEntity);
        var response = createdProduct.ToModel().ToResponse();
        return await ApplyDebugBugFilterAsync(response);
    }

    public async Task<bool> DeleteProduct(string id)
    {
        return await repo.DeleteAsync(id);
    }

    public async Task<MongoProductResponse?> GetProductById(string id)
    {
        var product = await repo.GetByIdAsync(id);
        if (product == null) return null;
        var response = product.ToModel().ToResponse();
        return await ApplyDebugBugFilterAsync(response);
    }

    public async Task<List<MongoProductResponse>> GetProducts()
    {
        var product = await repo.GetAllAsync();
        var responses = product.Select(p => p.ToModel().ToResponse()).ToList();
        return await ApplyDebugBugFilterAsync(responses);
    }

    public async Task<List<MongoProductResponse>> GetProductsByIdsAsync(List<string> ids)
    {
        var products = await repo.GetByIdsAsync(ids);
        var responses = products.Select(p => p.ToModel().ToResponse()).ToList();
        return await ApplyDebugBugFilterAsync(responses);
    }

    public async Task<List<MongoProductResponse>> GetProductsByTypeAsync(ProductTypeEnum productType)
    {
        var products = await repo.GetByTypeAsync(productType);
        var responses = products
            .Select(p => p.ToModel().ToResponse())
            .ToList();
        return await ApplyDebugBugFilterAsync(responses);
    }

    public async Task<MongoProductResponse?> UpdateProduct(string id, MongoProductRequest product)
    {
        var productModel = product.ToModel();
        productModel.Id = id;

        var productEntity = productModel.ToEntity();

        var updated = await repo.UpdateAsync(productEntity);

        if (!updated) return null;

        var response = productEntity.ToModel().ToResponse();
        return await ApplyDebugBugFilterAsync(response);
    }

    public async Task<T> ApplyDebugBugFilterAsync<T>(T product) where T : MongoProductResponse
    {
        if (product == null) return null;

        if (await debugService.GetStateAsync("brokenImages"))
        {
            if (product is ProductWithSizesResponse pwsr)
            {
                foreach (var color in pwsr.Kleuren)
                {
                    color.ImageUrl = null;
                }
            }
            else if (product is SimpleProductResponse spr)
            {
                foreach (var color in spr.Kleuren)
                {
                    color.ImageUrl = null;
                }
            }
        }
        return product;
    }

    public async Task<List<T>> ApplyDebugBugFilterAsync<T>(List<T> products) where T : MongoProductResponse
    {
        if (products == null || products.Count == 0) return products;

        if (await debugService.GetStateAsync("brokenImages"))
        {
            foreach (var product in products)
            {
                await ApplyDebugBugFilterAsync(product);
            }
        }
        return products;
    }
}
