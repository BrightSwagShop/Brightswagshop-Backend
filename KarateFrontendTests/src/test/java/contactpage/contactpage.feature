Feature: Contact Page - Smoke Tests

  Background:
    * driver baseUrl + '/contact'

  Scenario: Load page successfully
    * match driver.url contains '/contact'
    * waitFor('h1')

  Scenario: Navbar verification
    * waitFor("[data-testid='logo-link']")
    * waitFor("[data-testid='about-link']")
    * waitFor("[data-testid='contact-link']")
    * waitFor("[data-testid='cart-link']")

  Scenario: Heading text verification
    * def heading = waitFor('h1').text
    * match heading contains 'contact'

  Scenario: Phone number visibility
    * waitFor("a[href='tel:+3234508842']")

  Scenario: Email visibility
    * waitFor("a[href='mailto:info@brightest.be']")

  Scenario: Contact form exists
    * waitFor('form')

  Scenario: Form fields validation
    * waitFor("input[name='firstName']")
    * waitFor("input[name='lastName']")
    * waitFor("input[name='email']")
    * waitFor("input[name='phone']")
    * waitFor("textarea[name='message']")

  Scenario: Footer verification
    * waitFor("[data-testid='footer-logo-link']")
    * waitFor("[data-testid='footer-about-link']")
    * waitFor("[data-testid='footer-contact-link']")
    * waitFor("[data-testid='footer-linkedin']")
    * waitFor("[data-testid='footer-facebook']")
    * waitFor("[data-testid='footer-instagram']")
