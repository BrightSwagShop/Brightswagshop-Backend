# Brightswagshop Backend API Test Catalogue

This README replaces the earlier smoke-test notes. It is a deduplicated Gherkin specification for high-coverage API testing of `FakeWebShop.Api`.

Each backend area is assigned one primary test design technique so the same behavior is not repeated under multiple designs.

## Technique Map

- Use Case Testing: end-to-end business journeys across multiple endpoints.
- State Transition Testing: flows where the same resource changes state across calls.
- Equivalence Partitioning: valid vs invalid input classes, role classes, and lookup classes.
- Boundary Value Analysis: minimum, empty, and one-item boundaries where the API naturally exposes them.

## Coverage Scope

- Users: `POST /api/users/register`, `POST /api/users/login`, `GET /api/users/me`, `POST /api/users/favoriteToevoegen`, `POST /api/users/favoriteVerwijderen`
- Products: `GET /api/products`, `GET /api/products/{id}`, `POST /api/products`, `DELETE /api/products/{id}`, `GET /api/products/type/{slug}`, `POST /api/products/by-ids`
- Shopping carts: `POST /api/shoppingcarts`, `GET /api/shoppingcarts/user/{userId}`, `GET /api/shoppingcarts/session/{sessionId}`, `DELETE /api/shoppingcarts/{id}`, `POST /api/shoppingcarts/user/{userId}/items`, `PUT /api/shoppingcarts/user/{userId}/quantity`, `DELETE /api/shoppingcarts/user/{userId}/item`, `POST /api/shoppingcarts/{cartId}/apply-discount`
- Orders: `POST /api/orders`, `GET /api/orders/{id}`, `GET /api/orders/user/{userId}`, `POST /api/orders/from-cart/{userId}`
- Payments: `POST /api/payments/{orderId}/checkout`, `POST /api/webhooks/stripe`
- Discounts: `POST /api/discounts`, `GET /api/discounts`, `GET /api/discounts/{id}`, `PUT /api/discounts/{id}`, `DELETE /api/discounts/{id}`
- Catalog and support: `GET /api/categories`, `GET /api/producttypes`, `POST /api/images/upload`, `GET /api/debug/claims`, `GET /api/admins/admin-only`

## Gherkin Specification

### Use Case Testing

```gherkin
@usecase @users
Feature: Public user account journeys
	As a visitor or signed-in user
	I want to register, log in, and manage my own account data
	So that I can use the store as an authenticated customer

	Scenario: A visitor registers and then logs in successfully
		Given I have a valid new user registration payload
		When I POST "/api/users/register" with the registration payload
		Then the response status should be 200
		And the response should contain a user id
		And the response username should match the registration payload
		When I POST "/api/users/login" with the same credentials
		Then the response status should be 200
		And the response should contain a JWT token

	Scenario: An authenticated user retrieves their own profile
		Given I have a valid logged-in user token
		When I GET "/api/users/me" with the user token
		Then the response status should be 200
		And the response should contain the same user id as the token subject
		And the response username should match the logged-in user

	Scenario: An authenticated user adds and removes a favorite product
		Given I have a valid logged-in user token
		And I have an existing product id
		When I POST "/api/users/favoriteToevoegen" with the product id and user token
		Then the response status should be 200
		And the user should now contain the product in favorites
		When I POST "/api/users/favoriteVerwijderen" with the same product id and user token
		Then the response status should be 200
		And the user should no longer contain the product in favorites

	Scenario: An admin uploads an image and publishes a product
		Given I am authenticated as an admin user
		And I have a valid image file
		And I have a valid product payload
		When I POST "/api/images/upload" with the image file as admin
		Then the response status should be 200
		And the response should contain an imageUrl
		When I POST "/api/products" with the product payload as admin
		Then the response status should be 201
		And the created product should be retrievable by id
		When I DELETE the created product by id as admin
		Then the response status should be 204

	Scenario: An admin creates and updates a discount definition
		Given I am authenticated as an admin user
		And I have a valid discount payload
		When I POST "/api/discounts" with the discount payload as admin
		Then the response status should be 201
		And I remember the created discount id
		When I PUT "/api/discounts/{discountId}" with an updated discount payload
		Then the response status should be 200
		And the response should reflect the updated discount values

	Scenario: A customer creates an order from a cart and starts checkout
		Given I have a shopping cart with at least one item for the customer
		When I POST "/api/orders/from-cart/{userId}" for that customer
		Then the response status should be 201
		And I remember the created order id
		When I POST "/api/payments/{orderId}/checkout" for the created order
		Then the response status should be 200
		And the response should contain a payment session reference
```

### State Transition Testing

```gherkin
@state
Feature: Shopping cart and discount state transitions
	As a shopper
	I want cart mutations and discount application to move the cart through valid states
	So that cart changes remain consistent across successive requests

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

	Scenario: Applying a discount changes the cart and applying it again is rejected
		Given a shopping cart exists with at least one item
		And a valid discount code exists
		When I POST "/api/shoppingcarts/{cartId}/apply-discount" with the discount code
		Then the response status should be 200
		And the cart totals should reflect the discount
		When I POST "/api/shoppingcarts/{cartId}/apply-discount" with the same discount code again
		Then the response status should be 409
		And the cart should keep the first discount state
```

### Equivalence Partitioning

```gherkin
@ep @users
Feature: User input and authorization partitions
	As an API consumer
	I want valid and invalid user partitions to be handled consistently
	So that authentication and profile operations are predictable

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

@ep @catalog
Feature: Product lookup and catalog partitions
	As a storefront client
	I want product and catalog endpoints to separate valid and invalid lookups cleanly
	So that public browsing behaves predictably

	Scenario: Listing all products returns a populated collection partition
		When I GET "/api/products"
		Then the response status should be 200
		And the response should be an array of products

	Scenario Outline: Product lookup by id follows existing and missing id partitions
		When I GET "/api/products/<productId>"
		Then the response status should be <status>

		Examples:
			| productId                | status |
			| existing-product-id      | 200    |
			| 000000000000000000000000 | 404    |

	Scenario Outline: Product lookup by type follows known and unknown slug partitions
		When I GET "/api/products/type/<slug>"
		Then the response status should be <status>

		Examples:
			| slug    | status |
			| tshirt  | 200    |
			| hoodie  | 200    |
			| mok     | 200    |
			| sticker | 200    |
			| unknown | 404    |

	Scenario: Categories endpoint returns the category shape partition
		When I GET "/api/categories"
		Then the response status should be 200
		And every returned category should contain a numeric id and a string name

	Scenario: Product types endpoint returns the product type shape partition
		When I GET "/api/producttypes"
		Then the response status should be 200
		And every returned product type should contain a string name and a string slug

@ep @admin
Feature: Admin authorization partitions
	As a protected backend client
	I want role-based endpoints to distinguish anonymous, user, and admin access
	So that privileged actions are blocked from the wrong audience

	Scenario Outline: Product write operations follow role partitions
		Given I call the products endpoint with the "<auth>" authentication partition
		When I POST "/api/products" with a valid product payload
		Then the response status should be <createStatus>
		When I DELETE "/api/products/existing-product-id" with the same authentication partition
		Then the response status should be <deleteStatus>

		Examples:
			| auth         | createStatus | deleteStatus |
			| anonymous    | 401          | 401          |
			| regular user  | 403          | 403          |
			| admin user    | 201          | 204          |

	Scenario Outline: Image upload follows role and file-type partitions
		Given I call the image upload endpoint with the "<auth>" authentication partition
		And I attach a file in the "<fileType>" partition
		When I POST "/api/images/upload"
		Then the response status should be <status>

		Examples:
			| auth         | fileType     | status |
			| anonymous    | valid image  | 401    |
			| regular user | valid image  | 403    |
			| admin user   | valid image  | 200    |
			| admin user   | invalid type  | 400    |

	Scenario Outline: Discount creation follows role partitions
		Given I call the discounts endpoint with the "<auth>" authentication partition
		When I POST "/api/discounts" with a valid discount payload
		Then the response status should be <status>

		Examples:
			| auth         | status |
			| anonymous    | 401    |
			| regular user | 403    |
			| admin user   | 201    |

	Scenario Outline: Debug and admin-only endpoints follow authentication partitions
		Given I call the protected support endpoint with the "<auth>" authentication partition
		When I GET the protected endpoint
		Then the response status should be <status>

		Examples:
			| auth                  | status |
			| anonymous             | 401    |
			| valid HeaderAuth user | 200    |
			| valid HeaderAuth admin | 200    |
			| wrong role for admin api | 403  |

	Scenario Outline: Stripe webhook handling follows request shape partitions
		Given I prepare a Stripe webhook request in the "<partition>" partition
		When I POST "/api/webhooks/stripe"
		Then the response status should be <status>

		Examples:
			| partition             | status |
			| missing webhook secret | 400    |
			| missing signature      | 400    |
			| invalid signature      | 400    |
			| valid event            | 200    |

@ep @orders
Feature: Order lookup partitions
	As a customer or back-office client
	I want order read endpoints to separate existing and missing orders
	So that lookups return the right status without ambiguity

	Scenario Outline: Order lookup by id follows existing and missing partitions
		When I GET "/api/orders/<orderId>"
		Then the response status should be <status>

		Examples:
			| orderId                  | status |
			| existing-order-id        | 200    |
			| 000000000000000000000000 | 404    |

	Scenario Outline: Order lookup by user id follows existing and empty partitions
		When I GET "/api/orders/user/<userId>"
		Then the response status should be <status>

		Examples:
			| userId           | status |
			| existing-user-id | 200    |
			| missing-user-id  | 200    |
```

### Boundary Value Analysis

```gherkin
@bva @products
Feature: Minimum collection boundaries for product utilities
	As a client calling utility endpoints
	I want the API to handle zero and one item boundaries correctly
	So that edge-size requests are not misclassified

	Scenario Outline: Bulk product lookup treats an empty list and a single id as boundary values
		Given I prepare a product id list with <count> item(s)
		When I POST "/api/products/by-ids" with the id list
		Then the response status should be <status>

		Examples:
			| count | status |
			| 0     | 400    |
			| 1     | 200    |

	Scenario Outline: Image upload treats zero-byte and non-empty files as boundary values
		Given I prepare an upload file in the "<fileSize>" boundary state
		When I POST "/api/images/upload" as admin
		Then the response status should be <status>

		Examples:
			| fileSize  | status |
			| zero-byte | 400    |
			| minimal   | 200    |

	Scenario Outline: Order checkout treats empty and one-item orders as boundary values
		Given I prepare an order in the "<itemCount>" boundary state
		When I POST "/api/payments/<orderId>/checkout"
		Then the response status should be <status>

		Examples:
			| itemCount  | orderId              | status |
			| zero-items  | empty-order-id      | 400    |
			| one-item    | single-item-order-id | 200    |

	Scenario Outline: Header authentication treats missing and minimal user id values as boundary values
		Given I call a protected endpoint with X-User-Id value "<userIdHeader>"
		When I GET "/api/debug/claims"
		Then the response status should be <status>

		Examples:
			| userIdHeader | status |
			| missing      | 401    |
			| whitespace   | 401    |
			| u            | 200    |
```

## Notes for implementation

- These scenarios are intentionally grouped by primary technique, not by current test file structure.
- Replace placeholder ids such as `existing-product-id` and `existing-order-id` with seeded fixtures in the actual step definitions.
- The Gherkin above is a specification target for coverage planning; it is not a claim that the current `features/*.feature` files already contain all of these cases.
