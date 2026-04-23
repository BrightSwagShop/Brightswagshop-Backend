Feature: Discount API
  As a user
  I want to apply discount codes to my shopping cart
  So that I can receive price reductions and see correct totals

  Scenario: Only admins can create discounts
    Given I am authenticated as a regular user
    When I create a discount with code "NOADMIN20"
    Then I should receive a 403 Forbidden error

  Scenario: Admin can create discounts
    Given I am authenticated as an admin user
    When I create a discount with code "ADMIN20"
    Then I should receive a 201 Created response

  Scenario: Apply a valid discount code to a cart
    Given a shopping cart exists with user "user123" and product "productA"
    When I apply the discount code "SPRING20" to the cart
    Then the cart total should reflect the discount

  Scenario: Prevent applying a discount code twice
    Given a shopping cart exists with user "user123" and product "productA"
    And I apply the discount code "SPRING20" to the cart
    When I apply the discount code "SPRING20" to the cart again
    Then I should receive a 409 Conflict error

  Scenario: Apply an invalid discount code
    Given a shopping cart exists with user "user123" and product "productA"
    When I apply the discount code "INVALIDCODE" to the cart
    Then I should receive a 404 Not Found error
