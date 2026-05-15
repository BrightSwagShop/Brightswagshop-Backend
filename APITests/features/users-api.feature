Feature: Public user account journeys
	As a public website user
	I want to log in
  And land on the homepage after succesfull login
	So that I can use the store as an authenticated customer

  @HF
	Scenario: A visitor logs in successfully
		Given I have a valid user registration in MongoDB
		When I POST "/api/users/login" with the correct corresponding payload
		Then the response status should be 200
		And the response should contain a JWT token

  @HF
	Scenario: An authenticated user adds and removes a favorite product
		Given I have a valid logged-in user token
		And I have an existing product id
		When I POST "/api/users/favoriteToevoegen" with the product id and user token
		Then the response status should be 200
		And the user should now contain the product in favorites
		When I POST "/api/users/favoriteVerwijderen" with the same product id and user token
		Then the response status should be 200
		And the user should no longer contain the product in favorites

  @HF
	Scenario: A customer creates an order from a cart and starts checkout
		Given I have a shopping cart with at least one item for the customer
		When I POST "/api/orders/from-cart/{userId}" for that customer
		Then the response status should be 201
		And I remember the created order id
		When I POST "/api/payments/{orderId}/checkout" for the created order
		Then the response status should be 200
		And the response should contain a payment session reference

	Scenario Outline: Registering a user follows valid and invalid input partitions
		Given I prepare a registration payload in the "<partition>" partition
		When I POST "/api/users/register" with the registration payload
		Then the response status should be <status>

		Examples:
			| partition        | status |
			| valid            | 200    |
			| missing username | 400    |
			| missing password | 400    |
			| malformed body   | 400    |

	Scenario Outline: Logging in follows credential partitions
		Given I prepare login credentials in the "<partition>" partition
		When I POST "/api/users/login" with the login payload
		Then the response status should be <status>

		Examples:
			| partition      | status |
			| valid          | 200    |
			| unknown user   | 401    |
			| wrong password | 401    |
			| malformed body | 400    |

	Scenario Outline: Protected user endpoints follow authentication partitions
		Given I call the user endpoint with the "<auth>" authentication partition
		When I POST "/api/users/favoriteToevoegen" with a favorite payload
		Then the response status should be <status>

		Examples:
			| auth                   | status |
			| valid CustomJwt user   | 200    |
			| missing token          | 401    |
			| valid token wrong role | 403    |