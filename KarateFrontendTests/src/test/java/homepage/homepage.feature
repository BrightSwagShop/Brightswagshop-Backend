Feature: Homepage - Smoke Tests

  Background:
    * driver baseUrl + '/'

  Scenario: Load page successfully
    * match driver.url == baseUrl + '/'
    * waitFor('h1')

  Scenario: Navbar verification
    * waitFor("[data-testid='logo-link']")
    * waitFor("[data-testid='about-link']")
    * waitFor("[data-testid='contact-link']")
    * waitFor("[data-testid='cart-link']")

  Scenario: Heading text verification
    * def heading = waitFor('h1').text
    * match heading contains 'BrightSwagShop'

  Scenario: Footer verification
    * waitFor("[data-testid='footer-logo-link']")
    * waitFor("[data-testid='footer-about-link']")
    * waitFor("[data-testid='footer-contact-link']")
    * waitFor("[data-testid='footer-linkedin']")
    * waitFor("[data-testid='footer-facebook']")
    * waitFor("[data-testid='footer-instagram']")
