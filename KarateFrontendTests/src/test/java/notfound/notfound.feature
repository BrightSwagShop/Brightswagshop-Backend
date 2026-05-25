Feature: 404 Not Found Page - Smoke Tests

  Scenario: Load 404 page
    * driver baseUrl + '/nonexistent-page-12345'
    * match driver.url contains '/nonexistent-page-12345'

  Scenario: Display "Pagina niet gevonden" heading
    * driver baseUrl + '/nonexistent-page-12345'
    * def heading = waitFor('h1').text
    * match heading contains 'Pagina niet gevonden'

  Scenario: Display "Verder shoppen" link
    * driver baseUrl + '/nonexistent-page-12345'
    * waitFor('{a}Verder shoppen')

  Scenario: Click "Verder shoppen" redirects to home
    * driver baseUrl + '/nonexistent-page-12345'
    * click('{a}Verder shoppen')
    * match driver.url == baseUrl + '/'

  Scenario: Display error description text
    * driver baseUrl + '/nonexistent-page-12345'
    * def errorText = waitFor('p.text-gray-600').text
    * match errorText contains 'Deze pagina bestaat niet'
