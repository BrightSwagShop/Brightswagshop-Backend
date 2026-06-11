using BrightestSwagShop.Tests.Base;
using BrightestSwagShop.Tests.Helpers;
using BrightestSwagShop.Tests.Pages;
using NUnit.Framework;

namespace BrightestSwagShop.Tests.Tests
{
    public class ToggleTests : BaseTest
    {
        [Test]
        public async Task DisableAddToCart_Should_Be_Visible()
        {
            // test code
             var loginPage = new LoginPage(Driver);

            loginPage.GoToAdminPage(
                "admin@brightest.be",
                "password"
            );

            var togglePage = new TogglePage(Driver);

            Assert.That(togglePage.IsLoaded());

            var toggleHelper = new ToggleHelper();

            await toggleHelper.ToggleFeature(
                "DisableAddToCart"
            );

            Assert.Pass();
        }
        [Test]
        //checken of we het in backend niet andere naam heeft 
        public async Task AddToCart_Should_Be_Disabled_When_Bug_Is_Enabled()
        {
            var toggleHelper = new ToggleHelper();
            await toggleHelper.ToggleFeature("DisableAddToCart");

            Driver.Navigate().GoToUrl(
                "http://localhost:5173/products"
            );

            Assert.That(
                Driver.PageSource,
                Does.Contain("disabled")
            );
        }
    }

             
}