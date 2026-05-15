# Brightswagshop Backend API Negative Test Catalogue

This file lists negative Gherkin testcases (errors, invalid inputs, edge failures) for `FakeWebShop.Api`.

Guidelines:
- These scenarios focus on negative flows not duplicated in the main happy-flow catalogue.
- Grouped by primary test design technique: Use Case (negative journeys), State Transition (invalid state changes), Equivalence Partitioning (invalid partitions), Boundary Value Analysis (boundary error cases).

## Gherkin Negative Specification

### Use Case (Negative Journeys)

```gherkin
@usecase-negative @users
Feature: Failed account and order journeys
  Scenario: Registering an already-existing username is rejected
    Given a user already exists with username "existingUser"
    When I POST "/api/users/register" with a payload using username "existingUser"
    Then the response status should be 409
    And the response should contain an error indicating duplicate username

  Scenario: Protected AzureAd admin endpoint denies alternative auth schemes
    Given I am authenticated via HeaderAuth with role "App.Admin"
    When I GET "/api/admins/admin-only" with the HeaderAuth credentials
    Then the response status should be 401

  Scenario: Checkout fails when payment provider raises an error
    Given I have a created order with id "order-fails-on-provider"
    And the payment provider will simulate a service error for that order
    When I POST "/api/payments/order-fails-on-provider/checkout"
    Then the response status should be 500
    And the response should contain a payment service error message

  Scenario: Creating an order from a malformed cart fails
    Given the user's cart contains invalid/malformed item data
    When I POST "/api/orders/from-cart/{userId}"
    Then the response status should be 400
    And the response should explain the cart validation problem

  Scenario: Webhook endpoint receives malformed JSON payload
    Given I prepare an invalid JSON body for a Stripe webhook
    When I POST "/api/webhooks/stripe" with the malformed body and valid signature header
    Then the response status should be 400
```

### State Transition (Invalid/Conflicting State Changes)

```gherkin
@state-negative @shoppingcart
Feature: Invalid cart state transitions and conflict errors
  Scenario: Concurrent remove and update lead to not-found or conflict
    Given a cart exists with item "SKU-1" and quantity 1
    When two clients concurrently remove and update the same item
    Then one request should succeed and the other should return 404 or 409

  Scenario: Updating quantity to zero is rejected (invalid state)
    Given a shopping cart exists for user "u123" with item "SKU-2"
    When I PUT "/api/shoppingcarts/user/u123/quantity" with quantity 0
    Then the response status should be 400
    And the response should describe invalid quantity

  Scenario: Removing a non-existing cart item returns not found
    Given a shopping cart exists for user "u999" without item "missingSKU"
    When I DELETE "/api/shoppingcarts/user/u999/item" with product id "missingSKU"
    Then the response status should be 404

  Scenario: Applying a discount to a non-existent cart returns not found
    Given no cart exists with id "nonexistent-cart"
    When I POST "/api/shoppingcarts/nonexistent-cart/apply-discount" with code "SPRING"
    Then the response status should be 404

  Scenario: Discount application fails if cart no longer meets conditions
    Given a cart had enough total for discount but items were removed concurrently
    When I POST "/api/shoppingcarts/{cartId}/apply-discount" with the previously valid code
    Then the response status should be 409
    And the response should explain unmet conditions
```

### Equivalence Partitioning (Error Partitions not previously covered)

```gherkin
@ep-negative @ids @auth
Feature: Invalid id formats, bad tokens, and request shape partitions
  Scenario Outline: Malformed id format is rejected for id-based endpoints
    When I GET "<endpoint>" replacing <idPlaceholder> with "<malformedId>"
    Then the response status should be 400

    Examples:
      | endpoint                                   | idPlaceholder | malformedId       |
      | /api/products/<productId>                  | <productId>   | not-a-24-hex-id   |
      | /api/products/<productId>                  | <productId>   | 123               |
      | /api/orders/<orderId>                      | <orderId>     | order-!@#         |

  Scenario: JWTs signed with wrong key are rejected
    Given I have a JWT forged or signed with a wrong key
    When I GET "/api/users/me" with that token
    Then the response status should be 401

  Scenario: Expired JWT token is rejected
    Given I have a validly structured JWT that is expired
    When I GET "/api/users/me" with the expired token
    Then the response status should be 401

  Scenario: Product create payload with invalid enum/type values is rejected
    Given I am authenticated as admin
    When I POST "/api/products" with a payload that has productType "INVALID_TYPE"
    Then the response status should be 400
    And the response should describe the invalid enum/value

  Scenario: Product delete with non-existent but well-formed id returns 404
    Given I am authenticated as admin
    When I DELETE "/api/products/000000000000000000000001"
    Then the response status should be 404

  Scenario: Bulk product lookup with mixed valid and invalid id types handles errors predictably
    Given I prepare a list of ids containing both malformed and valid-looking ids
    When I POST "/api/products/by-ids" with that list
    Then the response status should be 400
    And the response should identify the invalid id entries

  Scenario: Create discount with invalid date range is rejected
    Given I am authenticated as admin
    When I POST "/api/discounts" with a discount where endDate < startDate
    Then the response status should be 400
```

### Boundary Value Analysis (Negative Boundaries)

```gherkin
@bva-negative @quantities @payloads
Feature: Boundary error cases for sizes, numbers and payload lengths
  Scenario Outline: Extremely large quantity values are rejected
    Given I am authenticated as user "u123"
    When I POST "/api/shoppingcarts/user/u123/items" with quantity <quantity>
    Then the response status should be <status>

    Examples:
      | quantity       | status |
      | -1             | 400    |
      | 0              | 200    |
      | 1000000000     | 400    |

  Scenario Outline: Oversized string fields are rejected for create endpoints
    Given I am authenticated as admin
    When I POST "<endpoint>" with a payload where the name/title field length is <length>
    Then the response status should be 400

    Examples:
      | endpoint                | length |
      | /api/products           | 10001  |
      | /api/discounts          | 10001  |

  Scenario: Uploading a zero-byte or corrupted file is rejected (already covered minimal cases) but also
    Given I am authenticated as admin
    When I POST "/api/images/upload" with a corrupted file stream
    Then the response status should be 400
    And the response should indicate upload or file parsing failure
```

## Implementation Notes

- These negatives avoid direct duplication of the scenarios present in `APITests/README.md` (happy flows and the earlier negative examples). Use the placeholder ids and behavior hints here when implementing step definitions.
- For deterministic server-side negative behaviors (e.g., payment provider failures, concurrent conflicts), implement test doubles or API hooks that simulate the failure modes.
- Replace placeholder ids like `order-fails-on-provider`, `nonexistent-cart`, `existingUser`, and `SKU-1` with seeded fixtures or factory-generated values in your step definitions.

---
Generated: negative-case catalogue — add step definitions to automate these.
