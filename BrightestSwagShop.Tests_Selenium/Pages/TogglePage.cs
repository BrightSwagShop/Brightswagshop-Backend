using OpenQA.Selenium;

namespace BrightestSwagShop.Tests.Pages;

public class TogglePage
{
    private readonly IWebDriver _driver;

    public TogglePage(IWebDriver driver)
    {
        _driver = driver;
    }

    public bool IsLoaded()
    {
        return _driver.PageSource.Contains("Bugs");
    }

    public bool HasToggle(string toggleName)
    {
        return _driver.PageSource.Contains(toggleName);
    }
}