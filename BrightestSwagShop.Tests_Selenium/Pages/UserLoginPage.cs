using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace BrightestSwagShop.Tests_Selenium.Pages;

public class UserLoginPage
{
    private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

    public UserLoginPage(IWebDriver driver)
        {
            _driver = driver;
              _wait = new WebDriverWait(
        driver,
        TimeSpan.FromSeconds(10)
    );
        }

        public void Login(
            string username,
            string password
        )
        {
            _driver.Navigate().GoToUrl(
                "http://localhost:5173/login"
            );

            _driver.FindElement(
                By.CssSelector(
                    "[data-testid='login-username-input']"
                )
            ).SendKeys(username);

            _driver.FindElement(
                By.CssSelector(
                    "[data-testid='login-password-input']"
                )
            ).SendKeys(password);

            _driver.FindElement(
                By.CssSelector(
                    "[data-testid='login-submit-button']"
                )
            ).Click();
        }

            public bool IsLoginFailed()
            {
                _wait.Until(d =>
                    d.PageSource.Contains("Login failed")
                );

                var errorMessage = _driver.FindElement(
                    By.XPath("//*[contains(text(),'Login failed')]")
                );

                return errorMessage.Displayed
                    && !_driver.Url.Contains("/home")
                    && !_driver.Url.Equals("http://localhost:5173/");
            }

            public void ClickLogout()
            {
                _driver.FindElement(
                    By.XPath("//button[contains(text(),'Logout')]")
                ).Click();
            }


            public bool IsLoginFailed1()
            {
                var error = _wait.Until(d =>
                    d.FindElement(
                        By.XPath("//*[contains(text(),'Login failed')]")
                    )
                );

                return error.Displayed;
            }
            public void ClickTshirt()
            {
                _wait.Until(d =>
                    d.FindElement(
                        By.XPath("//h2[contains(text(),'Hoodie')]")
                    )
                ).Click();
            }

              public void ClickTshirt1()
            {
                _wait.Until(d =>
                    d.FindElement(
                        By.XPath("//h2[contains(text(),'TShirt')]")
                    )
                ).Click();
            }



            public bool IsProductApiErrorVisible()
            {
                var error = _wait.Until(d =>
                    d.FindElement(
                        By.XPath("//*[contains(text(),'Producten konden niet geladen worden')]")
                    )
                );

                return error.Displayed;
            }

            public string GetTotalAmount()
            {
                return _driver.FindElement(
                    By.XPath("//p[text()='Totaal bedrag']/following-sibling::p")
                ).Text;
            }

            public string GetProductAmount()
            {
                return _driver.FindElement(
                    By.XPath("//p[contains(@class,'font-semibold') and contains(text(),'€')]")
                ).Text;
            }

            public bool HasWrongCartTotal()
            {
                return GetTotalAmount() != GetProductAmount();
            }

            public void ClickAddToCart()
            {
                var button = _wait.Until(d =>
                    d.FindElement(
                        By.XPath("//button[.//*[name()='svg'] and contains(.,'Add to cart')]")
                    )
                );

                button.Click();
            }


            public void OpenCart()
            {
                _wait.Until(d =>
                    d.FindElement(
                        By.CssSelector("[data-testid='cart-icon']")
                    )
                ).Click();
            }

            public bool IsFavoritesUnavailableVisible()
        {
            var error = _wait.Until(d =>
                d.FindElement(
                    By.XPath("//*[contains(text(),'Favorieten tijdelijk niet beschikbaar')]")
                )
            );

            return error.Displayed;
        }

        public void ClickFavoriteButton()
        {
        try
            {
                
                _wait.Until(d =>
                    d.FindElement(
                        By.CssSelector("button[aria-label='Voeg toe aan favorieten']")
                    )
                ).Click();
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void OpenFavorites()
        {
        try
            {
                
            _wait.Until(d =>
                    d.FindElement(
                        By.CssSelector("[data-testid='favorites-icon']")
                    )
                ).Click();

                Thread.Sleep(10000);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
}