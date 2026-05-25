Feature: Discount API
  API checks for discount creation and application.

  Background:
    * url baseUrl

  Scenario: Only admins can create discounts
    * def unique = java.lang.System.currentTimeMillis() + ''
    * def now = new java.util.Date()
    * def startsAt = new java.util.Date(now.getTime() - 3600000).toInstant().toString()
    * def endsAt = new java.util.Date(now.getTime() + 86400000).toInstant().toString()
    * configure headers = userHeaders
    Given path '/api/discounts'
    And request
      """
      {
        "name": "User Discount #(unique)",
        "description": "Should fail",
        "percentage": 10,
        "code": "#('NODMIN-' + unique)",
        "startsAt": "#(startsAt)",
        "endsAt": "#(endsAt)",
        "isActive": true
      }
      """
    When method POST
    Then status 403

  Scenario: Admin can create discounts
    * def unique = java.lang.System.currentTimeMillis() + ''
    * def now = new java.util.Date()
    * def startsAt = new java.util.Date(now.getTime() - 3600000).toInstant().toString()
    * def endsAt = new java.util.Date(now.getTime() + 86400000).toInstant().toString()
    * def discountCode = 'ADMIN-' + unique
    * configure headers = adminHeaders
    Given path '/api/discounts'
    And request
      """
      {
        "name": "Admin Discount #(unique)",
        "description": "Created by admin",
        "percentage": 20,
        "code": "#(discountCode)",
        "startsAt": "#(startsAt)",
        "endsAt": "#(endsAt)",
        "isActive": true
      }
      """
    When method POST
    Then status 201
    And match response.id == '#string'

    # Cleanup
    * def discountId = response.id
    Given path '/api/discounts/' + discountId
    When method DELETE
    Then status 204

  Scenario: Apply a valid discount code to a cart
    * def unique = java.lang.System.currentTimeMillis() + ''
    * def now = new java.util.Date()
    * def startsAt = new java.util.Date(now.getTime() - 3600000).toInstant().toString()
    * def endsAt = new java.util.Date(now.getTime() + 86400000).toInstant().toString()
    * def discountCode = 'VALID-' + unique

    # Seed product
    * configure headers = adminHeaders
    Given path '/api/products'
    And request
      """
      {
        "$type": "SimpleProduct",
        "name": "Discount Mug #(unique)",
        "description": "test",
        "price": 29.99,
        "category": "Drinkartikelen",
        "productType": "Mok",
        "isActive": true,
        "kleuren": [{ "kleur": "Zwart", "imageUrl": "https://example.com/x.png", "stock": 10, "sku": "#('DIS-' + unique)" }]
      }
      """
    When method POST
    Then status 201
    * def seededProductId = response.id

    # Seed discount
    Given path '/api/discounts'
    And request
      """
      {
        "name": "Test Discount #(unique)",
        "description": "Valid discount",
        "percentage": 20,
        "code": "#(discountCode)",
        "startsAt": "#(startsAt)",
        "endsAt": "#(endsAt)",
        "isActive": true
      }
      """
    When method POST
    Then status 201
    * def discountId = response.id

    # Create cart
    * configure headers = null
    Given path '/api/shoppingcarts'
    And request
      """
      {
        "userId": "karate-dis-user-#(unique)",
        "items": [{ "productId": "#(seededProductId)", "quantity": 1 }]
      }
      """
    When method POST
    Then status 201
    * def cartId = response.id

    # Apply discount
    Given path '/api/shoppingcarts/' + cartId + '/apply-discount'
    And request { code: '#(discountCode)' }
    When method POST
    Then status 200
    And match response.totalPrice == '#number'

    # Cleanup
    * configure headers = adminHeaders
    Given path '/api/shoppingcarts/' + cartId
    When method DELETE
    Then status 204

    Given path '/api/discounts/' + discountId
    When method DELETE
    Then status 204

    Given path '/api/products/' + seededProductId
    When method DELETE
    Then status 204

  Scenario: Prevent applying a discount code twice
    * def unique = java.lang.System.currentTimeMillis() + ''
    * def now = new java.util.Date()
    * def startsAt = new java.util.Date(now.getTime() - 3600000).toInstant().toString()
    * def endsAt = new java.util.Date(now.getTime() + 86400000).toInstant().toString()
    * def discountCode = 'TWICE-' + unique

    # Seed product
    * configure headers = adminHeaders
    Given path '/api/products'
    And request
      """
      {
        "$type": "SimpleProduct",
        "name": "Twice Mug #(unique)",
        "description": "test",
        "price": 29.99,
        "category": "Drinkartikelen",
        "productType": "Mok",
        "isActive": true,
        "kleuren": [{ "kleur": "Zwart", "imageUrl": "https://example.com/x.png", "stock": 10, "sku": "#('TWI-' + unique)" }]
      }
      """
    When method POST
    Then status 201
    * def seededProductId = response.id

    # Seed discount
    Given path '/api/discounts'
    And request
      """
      {
        "name": "Twice Discount #(unique)",
        "description": "test",
        "percentage": 10,
        "code": "#(discountCode)",
        "startsAt": "#(startsAt)",
        "endsAt": "#(endsAt)",
        "isActive": true
      }
      """
    When method POST
    Then status 201
    * def discountId = response.id

    # Create cart
    * configure headers = null
    Given path '/api/shoppingcarts'
    And request
      """
      {
        "userId": "karate-twice-user-#(unique)",
        "items": [{ "productId": "#(seededProductId)", "quantity": 1 }]
      }
      """
    When method POST
    Then status 201
    * def cartId = response.id

    # Apply discount first time
    Given path '/api/shoppingcarts/' + cartId + '/apply-discount'
    And request { code: '#(discountCode)' }
    When method POST
    Then status 200

    # Apply same discount second time → 409
    Given path '/api/shoppingcarts/' + cartId + '/apply-discount'
    And request { code: '#(discountCode)' }
    When method POST
    Then status 409

    # Cleanup
    * configure headers = adminHeaders
    Given path '/api/shoppingcarts/' + cartId
    When method DELETE
    Then status 204

    Given path '/api/discounts/' + discountId
    When method DELETE
    Then status 204

    Given path '/api/products/' + seededProductId
    When method DELETE
    Then status 204

  Scenario: Apply an invalid discount code
    * def unique = java.lang.System.currentTimeMillis() + ''

    # Seed product
    * configure headers = adminHeaders
    Given path '/api/products'
    And request
      """
      {
        "$type": "SimpleProduct",
        "name": "Invalid Disc Mug #(unique)",
        "description": "test",
        "price": 9.99,
        "category": "Drinkartikelen",
        "productType": "Mok",
        "isActive": true,
        "kleuren": [{ "kleur": "Zwart", "imageUrl": "https://example.com/x.png", "stock": 10, "sku": "#('INV-' + unique)" }]
      }
      """
    When method POST
    Then status 201
    * def seededProductId = response.id

    # Create cart
    * configure headers = null
    Given path '/api/shoppingcarts'
    And request
      """
      {
        "userId": "karate-inv-user-#(unique)",
        "items": [{ "productId": "#(seededProductId)", "quantity": 1 }]
      }
      """
    When method POST
    Then status 201
    * def cartId = response.id

    # Apply invalid discount code → 404
    Given path '/api/shoppingcarts/' + cartId + '/apply-discount'
    And request { code: 'DEFINITELY-INVALID-CODE-XYZ' }
    When method POST
    Then status 404

    # Cleanup
    * configure headers = adminHeaders
    Given path '/api/shoppingcarts/' + cartId
    When method DELETE
    Then status 204

    Given path '/api/products/' + seededProductId
    When method DELETE
    Then status 204
