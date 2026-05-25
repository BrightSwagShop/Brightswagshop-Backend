Feature: Navigation - Smoke Tests

  Background:
    * driver baseUrl + '/'

  Scenario: Navigate to Home via navbar logo
    * click("[data-testid='logo-link']")
    * match driver.url == baseUrl + '/'

  Scenario: Navigate to About page via navbar
    * click("[data-testid='about-link']")
    * match driver.url contains '/about'

  Scenario: Navigate to Contact page via navbar
    * click("[data-testid='contact-link']")
    * match driver.url contains '/contact'

  Scenario: Navigate to Cart via navbar
    * click("[data-testid='cart-link']")
    * match driver.url contains '/winkelwagen'

  Scenario: Navigate to Home via footer logo
    * click("[data-testid='footer-logo-link']")
    * match driver.url == baseUrl + '/'

  Scenario: Navigate to About page via footer
    * click("[data-testid='footer-about-link']")
    * match driver.url contains '/about'

  Scenario: Navigate to Contact page via footer
    * click("[data-testid='footer-contact-link']")
    * match driver.url contains '/contact'
