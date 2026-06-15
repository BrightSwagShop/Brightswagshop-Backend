using BrightestSwagShop.Tests.Pages;
using BrightestSwagShop.Tests_Selenium.Helpers;
using BrightestSwagShop.Tests_Selenium.Pages;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace BrightestSwagShop.Tests.Base
{
    public class BaseTest
    {
        [OneTimeSetUp]
        public void BeforeAllTests()
        {
            ReportManager.InitReport();
        }
        protected IWebDriver Driver;
        //browser openen en naar de site gaan.
        [SetUp]
        public void Setup()
        {
            Driver = new ChromeDriver();

            Driver.Manage().Window.Maximize();

            Driver.Navigate().GoToUrl(
                ConfigHelper.BaseUrl
            );
        }

        protected void LoginAsAdmin()
        {
            var loginPage =
                new LoginPage(Driver);

            loginPage.LoginAsAdmin(
                "emailadres admin",
                "wachtwoord"
            );

            loginPage.WaitForDashboard();
        }

        protected void LoginAsUser()
        {
            var userLogin =
                new UserLoginPage(Driver);

            userLogin.Login(
                "standaard_gebruiker",
                "Pass123!"
            );
        }
        //browser sluiten en opruimen.
       [TearDown]
        public void TearDown()
        {
            Driver?.Quit();
            Driver?.Dispose();
        }


        [OneTimeTearDown]
        public void AfterAllTests()
        {
            ReportManager.Flush();
              System.Diagnostics.Process.Start(
                    new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = "TestReport.html",
                        UseShellExecute = true
                    }
                );
        }
    }
}