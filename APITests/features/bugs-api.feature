Feature: Bug flags API
  Verify that activated debug bug flags produce the expected broken API behavior.
  Each scenario enables a bug, checks the broken behavior, then cleans up.

  Scenario: productApiError bug causes GET /api/products to return 500
    Given the "productApiError" debug bug is enabled
    When I request all products via the bug test context
    Then the bug test response status should be 500

  Scenario: brokenImages bug causes product image URLs to be null
    Given a test product is seeded for bug testing
    And the "brokenImages" debug bug is enabled
    When I request the seeded test product by id
    Then the bug test response status should be 200
    And all product color image URLs in the bug test response should be null

  Scenario: loginFails bug causes a valid login to return unauthorized
    Given a test user is registered for bug login testing
    And the "loginFails" debug bug is enabled
    When I login with the bug test user credentials
    Then the bug test response status should be 401

  Scenario: WrongCartTotal bug inflates cart total by 10
    Given a test product is seeded for bug testing
    And the "WrongCartTotal" debug bug is enabled
    When I create a cart with the seeded test product
    Then the bug test cart total should be the product price plus 10
