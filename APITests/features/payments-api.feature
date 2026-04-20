Feature: Payments API
  Smoke API checks for Stripe payment and webhook endpoints.

  @smoke @qase
  Scenario: Create checkout session for unknown order returns 404
    When I create checkout session for unknown order id
    Then the payments response status should be 404
    And the payments response should contain text "Order not found"

  @smoke @qase
  Scenario: Create checkout session for empty order returns 400
    Given I create an empty order for payments
    When I create checkout session for the stored payments order
    Then the payments response status should be 400
    And the payments response should contain text "Order has no items"

  @smoke @qase
  Scenario: Stripe webhook without signature returns 400
    When I POST Stripe webhook event without signature
    Then the payments response status should be 400
