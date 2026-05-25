Feature: Payments API
  Smoke API checks for Stripe payment and webhook endpoints.

  Background:
    * url baseUrl

  Scenario: Create checkout session for unknown order returns 404
    When path '/api/payments/000000000000000000000000/checkout'
    And method POST
    Then status 404
    And match response contains 'Order not found'

  Scenario: Create checkout session for empty order returns 400
    * def unique = java.lang.System.currentTimeMillis() + ''
    # Create empty order (no items)
    Given path '/api/orders'
    And request { userId: '#("order-karate-" + unique)', items: [] }
    When method POST
    Then status 201
    * def orderId = response.id

    # Checkout with empty order → 400
    Given path '/api/payments/' + orderId + '/checkout'
    When method POST
    Then status 400
    And match response contains 'Order has no items'

  Scenario: Stripe webhook without signature returns 400
    Given path '/api/webhooks/stripe'
    And request { id: 'evt_karate_test', type: 'checkout.session.completed' }
    When method POST
    Then status 400
