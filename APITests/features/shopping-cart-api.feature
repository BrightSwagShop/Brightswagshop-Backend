Feature: Shopping Cart API
  Smoke API checks for shopping cart create, retrieve, and delete flows.

  @smoke @qase
  Scenario: Create a cart for a user
    Given I prepare a shopping cart request for the smoke user with quantity 2
    When I create the shopping cart
    Then the shopping cart response status should be 201
    And the created shopping cart should match the smoke user
    And the created shopping cart should contain one item for seeded product with quantity 2
    And I remember the created shopping cart id

  @smoke @qase
  Scenario: Get cart by user id after creating one
    Given I prepare a shopping cart request for the smoke user with quantity 2
    And I create and remember a shopping cart
    When I get the shopping cart for the smoke user
    Then the shopping cart response status should be 200
    And the returned shopping cart should belong to the smoke user
    And the returned shopping cart should contain the seeded product

  @smoke @qase
  Scenario: Delete cart by id
    Given I prepare a shopping cart request for the delete user with quantity 1
    And I create and remember a shopping cart
    When I delete the remembered shopping cart
    Then the shopping cart response status should be 204
    When I get the shopping cart for the delete user
    Then the shopping cart response status should be 404
