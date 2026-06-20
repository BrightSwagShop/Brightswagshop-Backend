using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace BrightestSwagShop.Tests_Selenium.Pages
{
    public class AdminDashboardPage
    {
        private readonly IWebDriver _driver;
         private readonly WebDriverWait _wait;

    private readonly By _bugsButton =
        By.CssSelector("a[href='/admin/bugs']");

        public AdminDashboardPage(IWebDriver driver)
        {
            _driver = driver;
             _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        public void GoToBugsPage()
        {
                    _driver.Navigate().GoToUrl(
                "http://localhost:5173/admin/bugs"
            );

            Thread.Sleep(3000);

            Console.WriteLine(
                "Titel = " + _driver.Title
            );

            Console.WriteLine(
                "URL = " + _driver.Url
            );

            Console.WriteLine(
                _driver.PageSource
            );
        }
         public void ClickBugs()
        {
        //Selenium wacht totdat de Bugs-knop bestaat op de pagina.
            _wait.Until(d => d.FindElement(_bugsButton));
            //Zoekt de Bugs-knop en klikt erop.
             _driver.FindElement(_bugsButton).Click();
        }


 
    }
}