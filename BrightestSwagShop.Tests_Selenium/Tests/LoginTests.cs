using NUnit.Framework;
using BrightestSwagShop.Tests.Base;
using BrightestSwagShop.Tests.Pages;
using BrightestSwagShop.Tests_Selenium.Pages;
using BrightestSwagShop.Tests_Selenium.Helpers;
using OpenQA.Selenium;

namespace BrightestSwagShop.Tests_Selenium
{
    public class LoginTests : BaseTest
    {
        // [Test]
        // public void Admin_Can_Login()
        // {
        //  // Maak een LoginPage object aan zodat we login acties kunnen uitvoeren. 
        //     var loginPage = new LoginPage(Driver);
        //  // Log in als admin via Microsoft login.
        //     loginPage.LoginAsAdmin(
        //         "souline.asaad@brightest.be",
        //         "Schaatsen30.?"
        //     );

        //     // Toon de huidige URL in de console voor debugging.
        //     Console.WriteLine(
        //         "Na login = " + Driver.Url
        //     );
        //      // Maak een AdminDashboardPage object aan.
        //       var dashboardPage = new AdminDashboardPage(Driver);

        //     // Wacht totdat het dashboard volledig geladen is.
        //     loginPage.WaitForDashboard();
        //     // Klik op de Bugs knop in de navigatie.
        //     dashboardPage.ClickBugs();

        //      // Wacht 5 seconden zodat de bugs pagina volledig geladen is.
        //     Thread.Sleep(5000);
        //      // Maak een TogglePage object aan om met toggles te werken.
        //     var togglePage = new TogglePage(Driver);

        //    // Lees de status van de Wrong Cart Total toggle vóór de klik.
        //     var statusBefore = togglePage.GetWrongCartTotalStatus();
        //     Console.WriteLine(
        //         togglePage.GetWrongCartTotalStatus()
        //     );
        //     // Klik op de Wrong Cart Total toggle.

        //         // togglePage.ToggleWrongCartTotal(); werkt en nu 
        //         togglePage.ToggleBug(
        //            "Wrong cart total"
        //         );
        //           togglePage.ToggleBug(
        //            "Broken images"
        //         );
        //          togglePage.ToggleBug(
        //            "Broken favorites"
        //         );
        //          togglePage.ToggleBug(
        //            "Product API error"
        //         );
        //          togglePage.ToggleBug(
        //            "Slow loading"
        //         );
        //          togglePage.ToggleBug(
        //            "Login fails"
        //         );
        //          togglePage.ToggleBug(
        //            "Disable add to cart"
        //         );


        //         Thread.Sleep(1000);
        //         var status =
        //                 togglePage.GetBugStatus(
        //                     "Wrong cart total"
        //                 );

        //         // Lees de status opnieuw na de klik.

        //    // var statusAfter = togglePage.GetWrongCartTotalStatus();
        //       // Controleer dat de status veranderd is.
        //     // Bijvoorbeeld: Actief -> Niet actief.
        //     ReportManager.Test =
        //         ReportManager.Extent.CreateTest(
        //             "WrongCartTotal Toggle Test"
        //         );
        //     try
        //     {
                
        //         Assert.That(
        //             statusBefore,
        //             Is.Not.EqualTo(status)
        //         );


        //         ReportManager.Test.Pass(
        //             "Toggle succesvol gewijzigd"
        //         );
        //     }catch(Exception ex)
        //     {
                
        //       ReportManager.Test.Fail(
        //                 ex.Message
        //             );

        //             throw;
        //     }

         // Houd de browser nog 10 seconden open voor controle.

        //     Console.WriteLine(
        //         togglePage.GetWrongCartTotalStatus()
        //     );
        //       Thread.Sleep(10000);
        // }

       

        


        private void TestToggle(string toggleName)
        {
            ReportManager.Test =
                ReportManager.Extent.CreateTest(
                    toggleName
                );

            try
            {
                LoginAsAdmin();

                var dashboardPage =
                    new AdminDashboardPage(Driver);

                dashboardPage.ClickBugs();
                var paragraphs =
                    Driver.FindElements(By.TagName("p"));

                

                var togglePage =
                    new TogglePage(Driver);

                var before =
                    togglePage.GetBugStatus(toggleName);

                togglePage.ToggleBug(toggleName);
              
                var after =
                    togglePage.GetBugStatus(toggleName);

                Assert.That(
                    before,
                    Is.Not.EqualTo(after)
                );

                ReportManager.Test.Pass(
                    "Toggle succesvol gewijzigd"
                );  
                
            }
            catch(Exception ex)
            {
                ReportManager.Test.Fail(
                    ex.Message
                );

                throw;
            }
        }

        [Test]
        public void BrokenImages_Can_Be_Toggled()
        {
            TestToggle("Broken images");
        }

         
       

       [Test]
        public void SlowLoading_Can_Be_Toggled()
        {
            TestToggle("Slow loading");
        }

        [Test]
        public void BrokenFavorites_Can_Be_Toggled()
        {
            TestToggle("Broken favorites");
        }

        [Test]
        public void ProductApiError_Can_Be_Toggled()
        {
            TestToggle("Product API error");
        }

        [Test]
        public void DisableAddToCart_Can_Be_Toggled()
        {
            TestToggle("Disable add to cart");
        }

        [Test]
        public void WrongCartTotal_Can_Be_Toggled()
        {
            TestToggle("Wrong cart total");
        }




        [Test]
        public void LoginFails_Can_Be_Toggled()
        {
            TestToggle("Login fails");
        }

        [Test]
        public void DisableAddToCart_Disables_Button()
        {
              TestToggle("Disable add to cart");
           
            // dit kan ik in een reusable methode steken
            Driver.Manage().Cookies.DeleteAllCookies();

            // Driver.Navigate().GoToUrl(
            //     "http://localhost:5173/logout"
            // );

           LoginAsUser();
         Console.WriteLine(
                "Na user login = " + Driver.Url
            );
              Thread.Sleep(3000);

            var products =
                new ProductPage(Driver);
            Thread.Sleep(10000);
            products.Open();
            //Console.WriteLine(Driver.PageSource);
            Console.WriteLine(
                "Na products open = " + Driver.Url
            );
              Thread.Sleep(10000);

            Assert.That(
                products.IsAddToCartDisabled(),
                Is.True
            );
        }



       [Test]
        public void LoginFails_Prevents_User_Login()
        {
               ReportManager.Test =
                ReportManager.Extent.CreateTest(
                    "Login fails prevents user login"
                );
            try
            {   
            TestToggle("Login fails");

            var userLogin =
                new UserLoginPage(Driver);

             Driver.Manage().Cookies.DeleteAllCookies();
            
                Console.WriteLine(
                "Voor user login = " + Driver.Url
            );
            LoginAsUser();
            Assert.That(
                userLogin.IsLoginFailed(),
                Is.True
            );
             ReportManager.Test.Pass(
            "Standaard gebruiker kon niet inloggen en kreeg foutmelding."
             );
          
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

         [Test]
        public void APIFails()
        {
             ReportManager.Test =
                ReportManager.Extent.CreateTest(
                    "Products can't be shown- API error"
                );
             try
            {
                  
                
            TestToggle("Product API error");

            var userLogin =
                new UserLoginPage(Driver);

             Driver.Manage().Cookies.DeleteAllCookies();
                Console.WriteLine(
                    "Na logout = " + Driver.Url
                );
                 Thread.Sleep(3000);
                Console.WriteLine(
                "Voor user login = " + Driver.Url
            );
             LoginAsUser();
        //    Console.WriteLine(
        //         "Na user login = " + Driver.Url
        //     );
        //       Thread.Sleep(3000);
        //       var hoodies = Driver.FindElements(
        //         By.XPath("//h2[contains(text(),'Hoodie')]")
        //     );

        //     Console.WriteLine(
        //         "Aantal Hoodies gevonden: " + hoodies.Count
        //     );
        //     var links = Driver.FindElements(By.TagName("a"));

        //     foreach (var link in links)
        //     {
        //         Console.WriteLine(link.Text);
        //     }
            //Console.WriteLine(Driver.PageSource);
            userLogin.ClickTshirt1();
            Console.WriteLine("Current URL: " + Driver.Url);
            Console.WriteLine("Title: " + Driver.Title);

             Thread.Sleep(5000);




            Assert.That(
                userLogin.IsProductApiErrorVisible(),
                Is.True
            );

             ReportManager.Test.Pass(
            "Producten konden niet geladen worden en de gebruiker krijgt melding"
             );

             Thread.Sleep(10000);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }


          [Test]
        public void WrongCartTotal()
        {
             try
            {
                  
                
            TestToggle( "Wrong cart total");

             Driver.Manage().Cookies.DeleteAllCookies();
             Thread.Sleep(3000);
            var userLogin =
                new UserLoginPage(Driver);

                Console.WriteLine(
                    "Na logout = " + Driver.Url
                );
                 Thread.Sleep(3000);
                Console.WriteLine(
                "Voor user login = " + Driver.Url
            );
             LoginAsUser();
           Console.WriteLine(
                "Na user login = " + Driver.Url
            );
              Thread.Sleep(3000);
            //   var tshirts = Driver.FindElements(
            //     By.XPath("//h2[contains(text(),'TShirt')]")
            // );

            // Console.WriteLine(
            //     "Aantal tshirts gevonden: " + tshirts.Count
            // );
            // var links = Driver.FindElements(By.TagName("a"));

            // foreach (var link in links)
            // {
            //     Console.WriteLine(link.Text);
            // }
            //Console.WriteLine(Driver.PageSource);
            userLogin.ClickTshirt1();
            userLogin.ClickAddToCart();
            userLogin.OpenCart();
            Console.WriteLine("Current URL: " + Driver.Url);
            Console.WriteLine("Title: " + Driver.Title);
             Thread.Sleep(5000);
             //
            Assert.That(
                userLogin.HasWrongCartTotal(),
                Is.True
            );
             Thread.Sleep(10000);
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }

        [Test]
        public void BrokenFavorites_Shows_Error_Message()
        {
            ReportManager.Test =
                ReportManager.Extent.CreateTest(
                    "Favorites cannot be shown"
                );
            TestToggle("Broken favorites");

            Driver.Manage().Cookies.DeleteAllCookies();

            LoginAsUser();

            var userPage =
                new UserLoginPage(Driver);

            userPage.ClickTshirt1();
              Thread.Sleep(3000);
            userPage.ClickFavoriteButton();
              Thread.Sleep(3000);
            userPage.OpenFavorites();
            Console.WriteLine(
                "Current URL = " + Driver.Url
            );
            if(userPage.IsFavoritesUnavailableVisible())
            {
                
            Assert.That(
                userPage.IsFavoritesUnavailableVisible(),
                Is.True
            );
               ReportManager.Test.Pass(
            "Favoriten konden niet geladen worden en de gebruiker krijgt melding"
             );
            }
            else
            {
                  Assert.That(
                userPage.IsFavoritesUnavailableVisible(),
                Is.False
            );
               ReportManager.Test.Pass(
            "Favoriten konden wel geladen worden want toggle is uit"
             );
                
            }
        }
    }
}