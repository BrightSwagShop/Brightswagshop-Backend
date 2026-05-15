Feature: Discount CRUD validation
  As a user
  I want to apply discount codes to my shopping cart
  So that I can receive price reductions and see correct totals

  Scenario: Only admins can create discounts
    Given I am authenticated as a regular user for discount operations
    When I create a discount with code "NOADMIN20"
    Then I should receive a 403 Forbidden error

  @HF
  Scenario: Admin can create discounts
    Given I am authenticated as an admin user for discount operations
    When I create a discount with code "ADMIN20"
    Then I should receive a 201 Created response

  @HF
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
  
	Scenario: Applying a discount changes the cart and applying it again is rejected
		Given a shopping cart exists with at least one item
		And a valid discount code exists
		When I POST "/api/shoppingcarts/{cartId}/apply-discount" with the discount code
		Then the response status should be 200
		And the cart totals should reflect the discount
		When I POST "/api/shoppingcarts/{cartId}/apply-discount" with the same discount code again
		Then the response status should be 409
		And the cart should keep the first discount state

  Scenario: Applying a discount to a non-existent cart returns not found
    Given no cart exists with id "nonexistent-cart"
    When I POST "/api/shoppingcarts/nonexistent-cart/apply-discount" with code "SPRING"
    Then the response status should be 404

  Scenario: Create discount with invalid date range is rejected
    Given I am authenticated as admin
    When I POST "/api/discounts" with a discount where endDate < startDate
    Then the response status should be 400