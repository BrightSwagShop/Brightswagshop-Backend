using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FakeWebShop.Domain.Model.Discount;
using FakeWebShop.Domain.Model.Cart;
using Xunit;

namespace FakeWebShop.Api.Tests.Discount
{
    public class DiscountModelTests
    {
        [Fact]
        public void CalculateDiscountFor_ReturnsZero_WhenCartIsEmpty()
        {
            var discount = new DiscountModel { Percentage = 10, IsActive = true, StartsAt = DateTimeOffset.UtcNow.AddDays(-1) };
            var cart = new ShoppingCartModel { Items = new List<CartItemModel>() };
            var result = discount.CalculateDiscountFor(cart);
            Assert.Equal(0, result);
        }

        [Fact]
        public void CalculateDiscountFor_AppliesPercentageDiscount()
        {
            var discount = new DiscountModel { Percentage = 10, IsActive = true, StartsAt = DateTimeOffset.UtcNow.AddDays(-1) };
            var cart = new ShoppingCartModel
            {
                Items = new List<CartItemModel>
                {
                    new CartItemModel { ProductId = "1", ProductName = "A", UnitPrice = 100, Quantity = 1 }
                }
            };
            var result = discount.CalculateDiscountFor(cart);
            Assert.Equal(10, result);
        }

        [Fact]
        public void CalculateDiscountFor_RespectsStartAndEndDates()
        {
            var discount = new DiscountModel { Percentage = 10, IsActive = true, StartsAt = DateTimeOffset.UtcNow.AddDays(1) };
            var cart = new ShoppingCartModel
            {
                Items = new List<CartItemModel>
                {
                    new CartItemModel { ProductId = "1", ProductName = "A", UnitPrice = 100, Quantity = 1 }
                }
            };
            var result = discount.CalculateDiscountFor(cart);
            Assert.Equal(0, result);
        }
    }
}
