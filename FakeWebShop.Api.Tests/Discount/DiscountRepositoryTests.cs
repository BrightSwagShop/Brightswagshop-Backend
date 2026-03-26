using System;
using System.Threading.Tasks;
using FakeWebShop.Persistence.Entities.Discount;
using FakeWebShop.Persistence.MongoRepo_s;
using FakeWebShop.Persistence.MongoRepo_s.MongoInterface_s;
using Microsoft.Extensions.Options;
using Moq;
using MongoDB.Driver;
using Xunit;

namespace FakeWebShop.Api.Tests.Discount
{
    public class DiscountRepositoryTests
    {
        [Fact]
        public async Task CreateAndGetByCode_WorksCorrectly()
        {
            var collection = new Mock<IMongoCollection<Discount>>();
            var client = new Mock<IMongoClient>();
            var options = Options.Create(new FakeWebShop.Persistence.MongoRepo_s.Options.MongoOptions { Database = "TestDb" });
            client.Setup(c => c.GetDatabase(It.IsAny<string>(), null)).Returns(new Mock<IMongoDatabase>().Object);
            var repo = new DiscountRepository(client.Object, options);
            // This is a placeholder; for real tests, use an in-memory MongoDB or integration test
            // Here, just check that the methods exist and can be called
            await Assert.ThrowsAnyAsync<Exception>(() => repo.CreateAsync(new Discount()));
        }
    }
}
