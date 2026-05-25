Feature: About Page - Smoke Tests

  Background:
    * driver baseUrl + '/about'

  Scenario: Load page successfully
    * match driver.url contains '/about'
    * waitFor('h1')

  Scenario: Navbar verification
    * waitFor("[data-testid='logo-link']")
    * waitFor("[data-testid='about-link']")
    * waitFor("[data-testid='contact-link']")
    * waitFor("[data-testid='cart-link']")

  Scenario: Heading text verification
    * def heading = waitFor('h1').text
    * match heading contains 'Brightest'

  Scenario: Shop button visibility
    * waitFor("[data-testid='shop-button']")

  Scenario: Contact button visibility
    * waitFor("[data-testid='contact-button']")

  Scenario: Display customers section
    * waitFor('.grid')

  Scenario: Footer verification
    * waitFor("[data-testid='footer-logo-link']")
    * waitFor("[data-testid='footer-about-link']")
    * waitFor("[data-testid='footer-contact-link']")
    * waitFor("[data-testid='footer-linkedin']")
    * waitFor("[data-testid='footer-facebook']")
    * waitFor("[data-testid='footer-instagram']")
