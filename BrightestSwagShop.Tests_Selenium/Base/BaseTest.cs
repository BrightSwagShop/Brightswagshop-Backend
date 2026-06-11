using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace BrightestSwagShop.Tests.Base
{
    public class BaseTest
    {
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
        //browser sluiten en opruimen.
       [TearDown]
        public void TearDown()
        {
            Driver?.Quit();
            Driver?.Dispose();
        }
    }
}