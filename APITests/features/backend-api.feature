Feature: Backend API
  Basic API checks for categories, product types, and image upload validation.

  Scenario: Only admins can upload images
    Given I am authenticated as a regular user
    When I POST backend image upload as a regular user without file
    Then the backend response status should be 403

  @smoke @qase
  Scenario: GET categories returns 200 and list with id and name
    When I GET backend categories
    Then the backend response status should be 200
    And the backend response should be a non-empty array
    And the first backend item should contain number id and string name

  @smoke @qase
  Scenario: GET product types returns 200 and list with name and slug
    When I GET backend product types
    Then the backend response status should be 200
    And the backend response should be a non-empty array
    And the first backend item should contain string name and string slug

  @smoke @qase
  Scenario: POST image upload without file returns 400
    When I POST backend image upload without file
    Then the backend response status should be 400
