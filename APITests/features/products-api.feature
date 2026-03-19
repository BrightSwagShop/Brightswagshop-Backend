Feature: Products API
  Basic API checks for products endpoints.

  @smoke @QaseID=52
  Scenario: GET all products returns 200 and array
    When I GET "/api/products"
    Then the response status should be 200
    And the response should be an array

  @smoke @QaseID=53
  Scenario: GET unknown product returns 404
    When I GET "/api/products/000000000000000000000000"
    Then the response status should be 404

  @smoke @QaseID=54
  Scenario: Create, fetch and delete a product
    Given I have a valid mug payload
    When I POST "/api/products" with the payload
    Then the response status should be 201
    And I store the created product id
    When I GET the created product by id
    Then the response status should be 200
    When I DELETE the created product by id
    Then the response status should be 204
