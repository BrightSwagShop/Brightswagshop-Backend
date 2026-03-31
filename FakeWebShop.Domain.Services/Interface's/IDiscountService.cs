using System;
using FakeWebShop.Contracts.Request.DiscountRequest;
using FakeWebShop.Contracts.Response.DiscountResponse;

namespace FakeWebShop.Domain.Services.Interface_s;

public interface IDiscountService
{
    Task<DiscountResponse> CreateAsync(DiscountRequest request);
    Task<DiscountResponse?> GetByIdAsync(string id);
    Task<List<DiscountResponse>> GetAllAsync();
    Task<DiscountResponse?> UpdateAsync(string id, DiscountRequest request);
    Task<bool> DeleteAsync(string id);
}
