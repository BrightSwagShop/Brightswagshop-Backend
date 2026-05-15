Feature: Shopping Cart CRUD validation
  As a shopper
  I want to have a shopping cart assigned to me
  And be able to add and remove products from it
  So that is can shop on the website.

  @HF
  Scenario: Create a cart for a user
    Given I prepare a shopping cart request for the smoke user with quantity 2
    When I create the shopping cart
    Then the shopping cart response status should be 201
    And the created shopping cart should match the smoke user
    And the created shopping cart should contain one item for seeded product with quantity 2
    And I remember the created shopping cart id

  @HF
  Scenario: Get cart by user id after creating one
    Given I prepare a shopping cart request for the smoke user with quantity 2
    And I create and remember a shopping cart
    When I get the shopping cart for the smoke user
    Then the shopping cart response status should be 200
    And the returned shopping cart should belong to the smoke user
    And the returned shopping cart should contain the seeded product

  @HF
  Scenario Outline: Correct total price calculation
    Given I have <amount> <productId> in my cart
    And there is no active Discount
    When I GET my cart
    Then the cart totalPrice should be <amount> * <productPrice>

    Examples:
      | amount      | productId   | productPrice | totalPrice
      | 1           | 61549841    | 12.00        | 12.00
      | 10          | 5595897812  | 20.00        | 200.00
      | 20          | 895984955   | 30.00        | 600.00

  @HF
  Scenario: Delete product for cart by id
    Given I have a shopping cart with at least one product
    When I delete a product from my shoppingcart
    Then the shopping cart response status should be 200

  Scenario Outline: Extremely large quantity values are rejected
    Given I am authenticated as user "u123"
    When I POST "/api/shoppingcarts/user/u123/items" with quantity <quantity>
    Then the response status should be <status>

    Examples:
      | quantity       | status |
      | -1             | 400    |
      | 0              | 200    |
      | 1000000000     | 400    |

  @HF
	Scenario: A cart moves from empty to deleted through item mutations
		Given a fresh shopping cart exists for a user
		When I POST "/api/shoppingcarts/user/{userId}/items" with a product and quantity 1
		Then the cart should contain exactly one item
		When I PUT "/api/shoppingcarts/user/{userId}/quantity" with the same product and quantity 2
		Then the cart item quantity should be 2
		When I DELETE "/api/shoppingcarts/user/{userId}/item" with the same product
		Then the cart should no longer contain that product
		When I DELETE "/api/shoppingcarts/{cartId}"
		Then the response status should be 204
		And the cart should no longer exist
