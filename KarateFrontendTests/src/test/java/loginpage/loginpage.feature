Feature: Login Page - Smoke Tests

  Background:
    * driver baseUrl + '/login'

  Scenario: Load page successfully
    * match driver.url contains '/login'
    * waitFor("[data-testid='login-logo']")

  Scenario: Microsoft login button visibility
    * waitFor("[data-testid='microsoft-login-button']")
