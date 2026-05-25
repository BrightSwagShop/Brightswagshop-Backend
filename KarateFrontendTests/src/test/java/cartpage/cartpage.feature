Feature: Cart Page - Smoke Tests

  Background:
    * driver baseUrl + '/winkelwagen'

  Scenario: Load page successfully
    * match driver.url contains '/winkelwagen'

  Scenario: Navbar verification
    * waitFor("[data-testid='logo-link']")
    * waitFor("[data-testid='about-link']")
    * waitFor("[data-testid='contact-link']")
    * waitFor("[data-testid='cart-link']")

  Scenario: Error state shows "Terug naar home" button without authentication
    * waitFor('{button}Terug naar home')

  Scenario: Footer verification
    * waitFor("[data-testid='footer-logo-link']")
    * waitFor("[data-testid='footer-about-link']")
    * waitFor("[data-testid='footer-contact-link']")
    * waitFor("[data-testid='footer-linkedin']")
    * waitFor("[data-testid='footer-facebook']")
    * waitFor("[data-testid='footer-instagram']")
