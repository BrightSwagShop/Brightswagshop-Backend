Feature: Users API
  Smoke API checks for public user registration and login.

  Background:
    * url baseUrl

  Scenario: Register a new public user
    * def unique = java.lang.System.currentTimeMillis() + ''
    * def username = 'karate-user-' + unique
    * def password = 'P@ss-' + unique
    * def registerPayload = { username: '#(username)', password: '#(password)' }
    Given path '/api/users/register'
    And request registerPayload
    When method POST
    Then status 200
    And match response.id == '#string'
    And match response.username == username

  Scenario: Login with a registered public user
    * def unique = java.lang.System.currentTimeMillis() + ''
    * def username = 'karate-login-' + unique
    * def password = 'P@ss-' + unique
    * def creds = { username: '#(username)', password: '#(password)' }
    Given path '/api/users/register'
    And request creds
    When method POST
    Then status 200

    Given path '/api/users/login'
    And request creds
    When method POST
    Then status 200
    And match response.user.username == username

  Scenario: Login with unknown user returns 401
    Given path '/api/users/login'
    And request { username: 'no-such-karate-user-xyz', password: 'WrongPass999!' }
    When method POST
    Then status 401
