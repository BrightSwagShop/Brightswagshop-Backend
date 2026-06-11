using NUnit.Framework;
using BrightestSwagShop.Tests.Base;
using BrightestSwagShop.Tests.Pages;

namespace BrightestSwagShop.Tests_Selenium
{
    public class LoginTests : BaseTest
    {
        [Test]
        public void Admin_Can_Login()
        {
        //      Console.WriteLine("TEST START");
        //     var loginPage = new LoginPage(Driver);

        //     loginPage.GoToAdminPage(
        //         "souline.asaad@brightest.be",
        //         "GWcespCeN8aCxs6"
        //     );

        // //     Assert.That(
        // //         Driver.Url,
        // //         Does.Contain("/admin")
        // //     );
        //      Console.WriteLine("URL = " + Driver.Url);

        //     Assert.Fail("Stop hier");
          Driver.Navigate().GoToUrl("http://localhost:5173");

            var loginPage = new LoginPage(Driver);

            loginPage.LoginAsAdmin(
                "souline.asaad@brightest.be",
                "Schaatsen30.?"
            );

            Thread.Sleep(10000);
        }
    }
}