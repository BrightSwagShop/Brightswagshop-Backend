Feature: Payments CRUD validation
  Smoke API checks for Stripe payment and webhook endpoints.

  Scenario: Create checkout session for unknown order returns 404
    When I create checkout session for unknown order id
    Then the payments response status should be 404
    And the payments response should contain text "Order not found"

  Scenario: Create checkout session for empty order returns 400
    Given I create an empty order for payments
    When I create checkout session for the stored payments order
    Then the payments response status should be 400
    And the payments response should contain text "Order has no items"

  Scenario: Stripe webhook without signature returns 400
    When I POST Stripe webhook event without signature
    Then the payments response status should be 400

  @HF
  Scenario: Stripe webhook with valid order, session and signature returns 200
      When I POST Stripe webhook event with non empty cart
      And with valid checkout session
      And with correct signature
      Then the payments response status should be 200

  Scenario: Webhook endpoint receives malformed JSON payload
    Given I prepare an invalid JSON body for a Stripe webhook
    When I POST "/api/webhooks/stripe" with the malformed body and valid signature header
    Then the response status should be 400

  Scenario: Checkout fails when payment provider raises an error
    Given I have a created order with id "order-fails-on-provider"
    And the payment provider will simulate a service error for that order
    When I POST "/api/payments/order-fails-on-provider/checkout"
    Then the response status should be 500
    And the response should contain a payment service error message

  