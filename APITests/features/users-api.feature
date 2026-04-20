Feature: Users API
  Smoke API checks for public user registration and login.

  @smoke @qase
  Scenario: Register a new public user
    Given I prepare a unique user registration payload
    When I POST "/api/users/register" with the user payload
    Then the users response status should be 200
    And the users response should contain a user id
    And the users response username should match the request

  @smoke @qase
  Scenario: Login with a registered public user
    Given I register a unique public user
    When I POST "/api/users/login" with the same user credentials
    Then the users response status should be 200
    And the users response username should match the request

  @smoke @qase
  Scenario: Login with unknown user returns unauthorized
    Given I prepare credentials for an unknown public user
    When I POST "/api/users/login" with the user payload
    Then the users response status should be 401
