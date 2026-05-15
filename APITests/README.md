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



### Equivalence Partitioning

```gherkin



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

`

## Notes for implementation

- These scenarios are intentionally grouped by primary technique, not by current test file structure.
- Replace placeholder ids such as `existing-product-id` and `existing-order-id` with seeded fixtures in the actual step definitions.
- Any automated Newman request that creates data must delete that data in the same request flow or use a delete/revert endpoint immediately after the assertion.
- The positive Newman suite is cleanup-aware for carts, products, and discounts. User registration and order checkout stay in the specification only until the API exposes a safe teardown path.
- The Gherkin above is a specification target for coverage planning; it is not a claim that the current `features/*.feature` files already contain all of these cases.
