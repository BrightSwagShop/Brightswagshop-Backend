Feature: Discount API
  As a user
  I want to apply discount codes to my shopping cart
  So that I can receive price reductions and see correct totals

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
