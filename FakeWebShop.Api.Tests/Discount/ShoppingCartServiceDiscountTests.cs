using System;
using System.Threading.Tasks;
using FakeWebShop.Domain.Model.Cart;
using FakeWebShop.Domain.Model.Discount;
using FakeWebShop.Domain.Services;
using FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;
using Moq;
using Xunit;

namespace FakeWebShop.Api.Tests.Discount
{
    public class ShoppingCartServiceDiscountTests
    {
        [Fact]
        public async Task ApplyDiscountCodeAsync_AppliesDiscountAndPreventsDuplicate()
        {
            // Arrange
            var cart = new ShoppingCartModel
            {
                Id = "cart1",
                Items = [ new CartItemModel { ProductId = "1", ProductName = "A", UnitPrice = 100, Quantity = 1 } ],
                TotalPrice = 100
            };
            var discount = new DiscountModel { Percentage = 10, IsActive = true, StartsAt = DateTimeOffset.UtcNow.AddDays(-1) };
            var cartRepo = new Mock<IShoppingCartRepository>();
            var productRepo = new Mock<IMongoProductRepository>();
            var discountRepo = new Mock<IDiscountRepository>();
            cartRepo.Setup(r => r.GetByIdAsync("cart1")).ReturnsAsync(cart);
            discountRepo.Setup(r => r.GetByCodeAsync("CODE10")).ReturnsAsync(discount);
            cartRepo.Setup(r => r.UpdateAsync(It.IsAny<ShoppingCartModel>())).Returns(Task.CompletedTask);
            var service = new ShoppingCartService(cartRepo.Object, productRepo.Object, discountRepo.Object);

            // Act
            var response = await service.ApplyDiscountCodeAsync("cart1", "CODE10");

            // Assert
            Assert.NotNull(response);
            Assert.Equal(90, response.TotalPrice);

            // Try to apply again
            cart.DiscountApplied = true;
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.ApplyDiscountCodeAsync("cart1", "CODE10"));
        }
    }
}
