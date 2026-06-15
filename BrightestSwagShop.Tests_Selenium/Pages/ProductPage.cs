using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace BrightestSwagShop.Tests_Selenium.Pages;

public class ProductPage
{
    private readonly IWebDriver _driver;
    private readonly WebDriverWait _wait;
    public ProductPage(IWebDriver driver)
    {
        _driver = driver;
          _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
    }

    public void Open()
    {
        _driver.Navigate().GoToUrl(
            "http://localhost:5173/category/tshirt"
        );
    }

    public bool IsAddToCartDisabled()
    {
        _wait.Until(d =>
            !d.PageSource.Contains("Loading...")
        );

        var button = _driver.FindElement(
            By.XPath("//button[contains(.,'Add to cart')]")
        );

        return !button.Enabled;
    }
}