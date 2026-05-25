Feature: Products API
  Basic API checks for products endpoints.

  Background:
    * url baseUrl

  Scenario: GET all products returns 200 and array
    When path '/api/products'
    And method GET
    Then status 200
    And match response == '#[]'

  Scenario: GET unknown product returns 404
    When path '/api/products/000000000000000000000000'
    And method GET
    Then status 404

  Scenario: Create, fetch and delete a product
    * def unique = java.lang.System.currentTimeMillis() + ''
    * def productBody =
      """
      {
        "$type": "SimpleProduct",
        "name": "Karate Mug #(unique)",
        "description": "Karate test product",
        "price": 9.99,
        "category": "Drinkartikelen",
        "productType": "Mok",
        "isActive": true,
        "kleuren": [
          {
            "kleur": "Zwart",
            "imageUrl": "https://example.com/mug.png",
            "stock": 10,
            "sku": "#('KARAT-' + unique)"
          }
        ]
      }
      """
    # Create as admin
    * configure headers = adminHeaders
    Given path '/api/products'
    And request productBody
    When method POST
    Then status 201
    And match response.id == '#string'
    * def productId = response.id

    # Fetch anonymously
    * configure headers = null
    Given path '/api/products/' + productId
    When method GET
    Then status 200
    And match response.id == productId

    # Delete as admin
    * configure headers = adminHeaders
    Given path '/api/products/' + productId
    When method DELETE
    Then status 204

  Scenario: Only admins can create products
    * def unique = java.lang.System.currentTimeMillis() + ''
    * def productBody =
      """
      {
        "$type": "SimpleProduct",
        "name": "User Mug #(unique)",
        "description": "test",
        "price": 9.99,
        "category": "Drinkartikelen",
        "productType": "Mok",
        "isActive": true,
        "kleuren": [
          {
            "kleur": "Zwart",
            "imageUrl": "https://example.com/mug.png",
            "stock": 10,
            "sku": "#('USER-' + unique)"
          }
        ]
      }
      """
    * configure headers = userHeaders
    Given path '/api/products'
    And request productBody
    When method POST
    Then status 403

  Scenario: Only admins can delete products
    * configure headers = userHeaders
    Given path '/api/products/000000000000000000000000'
    When method DELETE
    Then status 403
