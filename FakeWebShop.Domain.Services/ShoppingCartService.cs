using System;
using FakeWebShop.Contracts.Request.CartRequest;
using FakeWebShop.Contracts.Response.CartResponse;
using FakeWebShop.Domain.Model.Cart;
using FakeWebShop.Domain.Services.Interface_s;
using FakeWebShop.Domain.Services.ServicesMapping.ShoppingCartMapping;
using FakeWebShop.Persistence.Entities;
using FakeWebShop.Persistence.Entities.BaseProduct;
using FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;

namespace FakeWebShop.Domain.Services;

public class ShoppingCartService(IShoppingCartRepository cartRepo, IMongoProductRepository productRepo) : IShoppingCartService
{
    public async Task<ShoppingCartResponse> CreateAsync(ShoppingCartRequest request)
    {
        var cartModel = request.AsModel();
        cartModel.UpdatedAt = DateTime.UtcNow;

        foreach (var item in cartModel.Items)
        {
            var product = await productRepo.GetByIdAsync(item.ProductId);

            if (product is null)
                throw new Exception($"Product with id {item.ProductId} not found.");

            item.ProductName = product.Name;
            item.UnitPrice = product.Price;
            item.ImageUrl = GetImageUrl(product, item.SelectedColor);
        }


        cartModel.TotalPrice = cartModel.Items.Sum(i => i.UnitPrice * i.Quantity);

        var entity = cartModel.AsEntity();

        await cartRepo.CreateAsync(entity);

        return entity.AsModel().AsResponse();
    }

    public async Task<bool> DeleteAsync(string id)
    {
        return await cartRepo.DeleteAsync(id);
    }

    public async Task<ShoppingCartResponse?> GetBySessionIdAsync(string sessionId)
    {
        var entity = await cartRepo.GetBySessionIdAsync(sessionId);

        if (entity is null)
            return null;

        return entity.AsModel().AsResponse();
    }

    public async Task<ShoppingCartResponse?> GetByUserIdAsync(string userId)
    {
        var entity = await cartRepo.GetByUserIdAsync(userId);

        if (entity is null)
            return null;

        return entity.AsModel().AsResponse();
    }

    // Update Aantal van een existing Item
    public async Task<ShoppingCartResponse?> UpdateQuantityAsync(string userId, CartItemRequest request)
    {
        var entity = await cartRepo.GetByUserIdAsync(userId);

        if (entity is null)
            return null;

        var cartModel = entity.AsModel();

        var item = cartModel.Items.FirstOrDefault(i =>
            i.ProductId == request.ProductId &&
            i.SelectedColor == request.SelectedColor);

        if (item is null)
            return null;

        item.Quantity = request.Quantity;


        cartModel.TotalPrice = cartModel.Items.Sum(i => i.UnitPrice * i.Quantity);
        cartModel.UpdatedAt = DateTime.UtcNow;

        var updatedEntity = cartModel.AsEntity();

        await cartRepo.UpdateAsync(updatedEntity);

        return updatedEntity.AsModel().AsResponse();
    }

    public async Task<ShoppingCartResponse?> RemoveItemAsync(string userId, CartItemRequest request)
    {
        var entity = await cartRepo.GetByUserIdAsync(userId);

        if (entity is null) return null;

        var cartModel = entity.AsModel();

        var item = cartModel.Items.FirstOrDefault(i =>
        i.ProductId == request.ProductId && i.SelectedColor == request.SelectedColor);

        if (item is null) return null;

        cartModel.Items.Remove(item);

        cartModel.TotalPrice = cartModel.Items.Sum(i => i.UnitPrice * i.Quantity);
        cartModel.UpdatedAt = DateTime.UtcNow;

        var updatedEntity = cartModel.AsEntity();

        await cartRepo.UpdateAsync(updatedEntity);

        return updatedEntity.AsModel().AsResponse();
    }

    public async Task<ShoppingCartResponse> AddItemAsync(string userId, CartItemRequest request)
    {
        var existingCartEntity = await cartRepo.GetByUserIdAsync(userId);

        var cartModel = existingCartEntity?.AsModel() ?? CreateNewCart(userId);

        var product = await productRepo.GetByIdAsync(request.ProductId);

        if (product is null)
            throw new Exception($"Product with id {request.ProductId} not found.");

        var existingItem = FindExistingItem(cartModel, request);

        if (existingItem is not null)
        {
            existingItem.Quantity += request.Quantity;
        }
        else
        {
            cartModel.Items.Add(CreateCartItemModel(product, request));
        }

        cartModel.TotalPrice = CalculateTotalPrice(cartModel);
        cartModel.UpdatedAt = DateTime.UtcNow;

        var updatedCartEntity = cartModel.AsEntity();

        if (existingCartEntity is null)
            await cartRepo.CreateAsync(updatedCartEntity);
        else
            await cartRepo.UpdateAsync(updatedCartEntity);

        return updatedCartEntity.AsModel().AsResponse();
    }

    // Helper method voor CreateAsync
    private static string GetImageUrl(Product product, string? selectedColor)
    {
        return product switch
        {
            ClothingProduct clothing => clothing.Kleuren
                .FirstOrDefault(k => k.Kleur == selectedColor)?.ImageUrl
                ?? clothing.Kleuren.FirstOrDefault()?.ImageUrl
                ?? string.Empty,

            MugProduct mug => mug.Kleuren
                .FirstOrDefault(k => k.Kleur == selectedColor)?.ImageUrl
                ?? mug.Kleuren.FirstOrDefault()?.ImageUrl
                ?? string.Empty,

            _ => string.Empty
        };
    }

    // Helper method voor AddItemAsync
    private static ShoppingCartModel CreateNewCart(string userId)
    {
        return new ShoppingCartModel
        {
            UserId = userId,
            UpdatedAt = DateTime.UtcNow
        };
    }

    private static CartItemModel? FindExistingItem(ShoppingCartModel cartModel, CartItemRequest request)
    {
        return cartModel.Items.FirstOrDefault(i =>
            i.ProductId == request.ProductId &&
            i.SelectedColor == request.SelectedColor);
    }

    private CartItemModel CreateCartItemModel(Product product, CartItemRequest request)
    {
        return new CartItemModel
        {
            ProductId = product.Id,
            ProductName = product.Name,
            SelectedColor = request.SelectedColor,
            ImageUrl = GetImageUrl(product, request.SelectedColor),
            UnitPrice = product.Price,
            Quantity = request.Quantity
        };
    }

    private static decimal CalculateTotalPrice(ShoppingCartModel cartModel)
    {
        return cartModel.Items.Sum(i => i.UnitPrice * i.Quantity);
    }


}



