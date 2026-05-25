Feature: Backend API
  Basic API checks for categories, product types, and image upload validation.

  Background:
    * url baseUrl

  Scenario: GET categories returns 200 and list with id and name
    When path '/api/categories'
    And method GET
    Then status 200
    And match response == '#[]'
    And match response[0].id == '#number'
    And match response[0].name == '#string'

  Scenario: GET product types returns 200 and list with name and slug
    When path '/api/producttypes'
    And method GET
    Then status 200
    And match response == '#[]'
    And match response[0].name == '#string'
    And match response[0].slug == '#string'

  Scenario: Only admins can upload images
    * configure headers = userHeaders
    Given path '/api/images/upload'
    When method POST
    Then status 403

  Scenario: POST image upload without file returns 400
    * configure headers = adminHeaders
    Given path '/api/images/upload'
    When method POST
    Then status 400
