using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace BrightestSwagShop.Tests.Pages
{
    public class LoginPage
    {
        private readonly IWebDriver _driver;
        private readonly WebDriverWait _wait;

        public LoginPage(IWebDriver driver)
        {
            _driver = driver;
            _wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }
        private IWebElement LoginButton =>
        _driver.FindElement(
            By.XPath("//button[contains(text(),'Login')]")
        );
        private IWebElement MicrosoftLoginButton =>
        _driver.FindElement(
            By.CssSelector("[data-testid='microsoft-login-button']")
        );

        private void OpenMicrosoftLogin()
        {
            LoginButton.Click();

            _wait.Until(d =>
                d.FindElement(
                    By.CssSelector(
                        "[data-testid='microsoft-login-button']"
                    )
                ));

            MicrosoftLoginButton.Click();
            Console.WriteLine("URL = " + _driver.Url);
        }

        public void LoginAsAdmin(string email, string password)
        {
            Console.WriteLine("LoginAsAdmin gestart");
            if (IsAlreadyLoggedIn())
                return;
            Console.WriteLine(IsAlreadyLoggedIn());
             OpenMicrosoftLogin();
            HandlePickAnAccount();

            if (ElementExists(By.Id("i0116")))
            {
                 Console.WriteLine("Email gevonden");
                EnterEmail(email);
                ClickNext();
                Console.WriteLine("Email invullen");
                Console.WriteLine("Next geklikt");

                _wait.Until(d =>
                    d.FindElement(By.Id("i0118")));

                Console.WriteLine("Password pagina geladen");
            }
            else
            {
                 Console.WriteLine("Email niet gevonden");
                
            }

            if (ElementExists(By.Id("i0118")))
            {
                Console.WriteLine("Password gevonden");
                EnterPassword(password);
                Thread.Sleep(2000);
                ClickNext();
                 Console.WriteLine("Password invulled");
            }
            else
            {
                Console.WriteLine("password niet gevonden");
            }

            Console.WriteLine("Login knop klikken");
            HandleStaySignedIn();
            Console.WriteLine(
                "URL na Yes = " + _driver.Url
            );
            Thread.Sleep(5000);
        }

        private void EnterEmail(string email)
        {
            _driver.FindElement(By.Id("i0116")).SendKeys(email);
        }

        private void EnterPassword(string password)
        {
            _driver.FindElement(By.Id("i0118")).SendKeys(password);
        }
        public void WaitForDashboard()
        {
            _wait.Until(d =>
                d.Url.Contains("/admin/dashboard"));
        }

        private void ClickNext()
        {
            // _driver.FindElement(By.Id("idSIButton9")).Click();
             _wait.Until(d =>
                            d.FindElement(By.Id("idSIButton9"))
                        ).Click();
        }

        private void HandlePickAnAccount()
        {
            // if (ElementExists(By.XPath("//*[contains(text(),'Use another account')]")))
            // {
            //     _driver.FindElement(
            //         By.XPath("//*[contains(text(),'Use another account')]")
            //     ).Click();
            // }
              Console.WriteLine("HandlePickAnAccount gestart");

            Thread.Sleep(5000);
        }

        private void HandleStaySignedIn()
        {
            // if (ElementExists(By.Id("idSIButton9")))
            // {
            //      Console.WriteLine("Stay signed in scherm gevonden");
            //     try
            //     {
            //         var button = _driver.FindElement(By.Id("idSIButton9"));

            //         if (button.Displayed)
            //         {
            //             button.Click();
            //          Console.WriteLine("Yes geklikt");
            //           Thread.Sleep(10000);
            //         }
            //     }
            //     catch
            //     {
            //          Console.WriteLine(
            //                 "Geen Stay signed in scherm gevonden"
            //             );
            //     }
            // }
               Console.WriteLine("HandleStaySignedIn gestart");

                    try
                    {
                        Thread.Sleep(3000);

                        var button = _driver.FindElement(
                            By.Id("idSIButton9"));

                        Console.WriteLine(
                            "Button value = " +
                            button.GetAttribute("value"));

                        button.Click();

                        Console.WriteLine("Button geklikt");

                        Thread.Sleep(10000);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
        }

        private bool IsAlreadyLoggedIn()
        {
            return _driver.Url.Contains("admin")
                   || _driver.PageSource.Contains("Feature Toggles")
                   || _driver.PageSource.Contains("Admin");
        }

        private bool ElementExists(By by)
        {
            try
            {
                return _driver.FindElements(by).Count > 0;
            }
            catch
            {
                return false;
            }
        }
        public void GoToAdminPage(string email, string password)
        {
            // _driver.Navigate().GoToUrl("http://localhost:5173/admin/bugs");

            // if (IsOnTogglePage())
            //     return;

            // LoginAsAdmin(email, password);

            // _driver.Navigate().GoToUrl("http://localhost:5173/admin/bugs");

            // _wait.Until(d =>
            //     d.Url.Contains("/admin/bugs"));
              LoginAsAdmin(email, password);
        }
        private bool IsOnTogglePage()
        {
            try
            {
                return _driver.Url.Contains("/admin/bugs");
            }
            catch
            {
                return false;
            }
        }

       public void ClickLogout()
        {
            _wait.Until(d =>
                d.FindElement(By.XPath("//button[./svg]"))
            ).Click();
        }
















           
     }
}