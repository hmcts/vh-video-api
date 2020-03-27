Feature: Conferences Today
  In order to focus on conferences today
  As an API service
  I want to view conferences for today

  Scenario: Get all conferences today
    Given I have several conferences
    And I have a valid get conferences for today request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And only todays conferences should be retrieved
  
  Scenario: Get list of conferences for judge with valid username
    Given I have a conference
    And I have a get conferences for a judge by username request with a valid username
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the summary of conference details should be retrieved

  Scenario: Get list of conferences for judge with an non-existent username
    Given I have several conferences
    And I have a get conferences for a judge by username request with a nonexistent username
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And an empty list is retrieved

  Scenario: Get list of conferences for judge with an invalid username
    Given I have a get conferences for a judge by username request with an invalid username
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should contain 'Please provide a valid username'


  Scenario: Get list of conferences for individual with valid username
    Given I have a conference
    And I have a get conferences for an individual by username request with a valid username
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the summary of conference details should be retrieved

  Scenario: Get list of conferences for individual with an non-existent username
    Given I have several conferences
    And I have a get conferences for an individual by username request with a nonexistent username
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And an empty list is retrieved

  Scenario: Get list of conferences for individual with an invalid username
    Given I have a get conferences for an individual by username request with an invalid username
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should contain 'Please provide a valid username'
