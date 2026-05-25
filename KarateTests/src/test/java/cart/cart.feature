Feature: Shopping Cart API
  Smoke API checks for shopping cart create, retrieve, and delete flows.

  Background:
    * url baseUrl

  Scenario: Create a cart for a user
    # Seed a product (admin)
    * def unique = java.lang.System.currentTimeMillis() + ''
    * def productBody =
      """
      {
        "$type": "SimpleProduct",
        "name": "Cart Seed Mug #(unique)",
        "description": "Seeded for cart test",
        "price": 9.99,
        "category": "Drinkartikelen",
        "productType": "Mok",
        "isActive": true,
        "kleuren": [{ "kleur": "Zwart", "imageUrl": "https://example.com/x.png", "stock": 10, "sku": "#('CART-' + unique)" }]
      }
      """
    * configure headers = adminHeaders
    Given path '/api/products'
    And request productBody
    When method POST
    Then status 201
    * def seededProductId = response.id

    # Create cart
    * def userId = 'karate-smoke-' + unique
    * configure headers = null
    Given path '/api/shoppingcarts'
    And request
      """
      {
        "userId": "#(userId)",
        "items": [{ "productId": "#(seededProductId)", "quantity": 2 }]
      }
      """
    When method POST
    Then status 201
    And match response.userId == userId
    And match response.id == '#string'
    And match response.items == '#[1]'
    And match response.items[0].productId == seededProductId
    And match response.items[0].quantity == 2

    # Cleanup
    * def cartId = response.id
    * configure headers = adminHeaders
    Given path '/api/shoppingcarts/' + cartId
    When method DELETE
    Then status 204

    * configure headers = adminHeaders
    Given path '/api/products/' + seededProductId
    When method DELETE
    Then status 204

  Scenario: Get cart by user id after creating one
    * def unique = java.lang.System.currentTimeMillis() + ''
    * def productBody =
      """
      {
        "$type": "SimpleProduct",
        "name": "Get Cart Mug #(unique)",
        "description": "Seeded for get cart test",
        "price": 9.99,
        "category": "Drinkartikelen",
        "productType": "Mok",
        "isActive": true,
        "kleuren": [{ "kleur": "Zwart", "imageUrl": "https://example.com/x.png", "stock": 10, "sku": "#('GETC-' + unique)" }]
      }
      """
    * configure headers = adminHeaders
    Given path '/api/products'
    And request productBody
    When method POST
    Then status 201
    * def seededProductId = response.id

    * def userId = 'karate-get-user-' + unique
    * configure headers = null
    Given path '/api/shoppingcarts'
    And request
      """
      {
        "userId": "#(userId)",
        "items": [{ "productId": "#(seededProductId)", "quantity": 1 }]
      }
      """
    When method POST
    Then status 201
    * def cartId = response.id

    # Get by user id
    Given path '/api/shoppingcarts/user/' + userId
    When method GET
    Then status 200
    And match response.userId == userId
    And match response.items[0].productId == seededProductId

    # Cleanup
    * configure headers = adminHeaders
    Given path '/api/shoppingcarts/' + cartId
    When method DELETE
    Then status 204

    Given path '/api/products/' + seededProductId
    When method DELETE
    Then status 204

  Scenario: Delete cart by id
    * def unique = java.lang.System.currentTimeMillis() + ''
    * def productBody =
      """
      {
        "$type": "SimpleProduct",
        "name": "Del Cart Mug #(unique)",
        "description": "Seeded for delete cart test",
        "price": 9.99,
        "category": "Drinkartikelen",
        "productType": "Mok",
        "isActive": true,
        "kleuren": [{ "kleur": "Zwart", "imageUrl": "https://example.com/x.png", "stock": 10, "sku": "#('DELC-' + unique)" }]
      }
      """
    * configure headers = adminHeaders
    Given path '/api/products'
    And request productBody
    When method POST
    Then status 201
    * def seededProductId = response.id

    * def userId = 'karate-del-user-' + unique
    * configure headers = null
    Given path '/api/shoppingcarts'
    And request
      """
      {
        "userId": "#(userId)",
        "items": [{ "productId": "#(seededProductId)", "quantity": 1 }]
      }
      """
    When method POST
    Then status 201
    * def cartId = response.id

    # Delete cart
    Given path '/api/shoppingcarts/' + cartId
    When method DELETE
    Then status 204

    # Verify gone (404)
    Given path '/api/shoppingcarts/user/' + userId
    When method GET
    Then status 404

    # Cleanup product
    * configure headers = adminHeaders
    Given path '/api/products/' + seededProductId
    When method DELETE
    Then status 204
