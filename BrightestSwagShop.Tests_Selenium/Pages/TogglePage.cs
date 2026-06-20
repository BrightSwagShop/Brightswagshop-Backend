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
    //Deze methode zoekt de toggle van Wrong cart total en klikt erop.
    public void ToggleWrongCartTotal()
    {
        var button = _driver.FindElement(
            By.XPath("//p[contains(text(),'Wrong cart total')]/following-sibling::button")
        );

        button.Click();
    }
    //Deze methode zoekt de toggle op basis van de naam van de bug en klikt vervolgens op de bijhorende aan/uit-knop.
    public void ToggleBug(string bugName)
    {
        var bugText = _driver.FindElement(
            By.XPath($"//p[contains(text(),'{bugName}')]")
        );

        var toggleButton = bugText.FindElement(
            By.XPath("./following-sibling::button")
        );

        toggleButton.Click();
    }
    //Deze methode haalt de huidige status van de "Wrong cart total" toggle op, bijvoorbeeld "Actief" of "Niet actief"
    public string GetWrongCartTotalStatus()
        {
            return _driver.FindElement(
                By.XPath("//p[contains(text(),'Wrong cart total')]/following-sibling::p")
            ).Text;
        }
        //Zoekt een bug op naam en geeft de huidige status terug, bijvoorbeeld "Actief" of "Niet actief".
        public string GetBugStatus(string bugName)
        {
            return _driver.FindElement(
                By.XPath($"//p[contains(text(),'{bugName}')]/following-sibling::p")
            ).Text;
        }
 
}