Feature: Conference
  In order to manage conferences
  As an api service
  I want to be able to create, retrieve, update and delete conferences

  Scenario: Get conference details by username
    Given I have a conference
    And The conference has a pending task
    And I have a get details for a conference request by username with a valid username
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the summary of conference details should be retrieved

  Scenario: Update conference
    Given I have a conference
    And I have an update conference request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the conference details have been updated

  Scenario: Create conference
    Given I have a valid book a new conference request
    When I send the request to the endpoint
    Then the response should have the status Created and success status True
    And the conference details should be retrieved

  Scenario: Get conference details
    Given I have a conference
    And I have a get details for a conference request with a valid conference id
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the conference details should be retrieved

  Scenario: Delete conference
    Given I have a conference
    And I have a valid delete conference request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the conference should be removed

  Scenario: Get conference details for todays hearings
    Given I have a conference
    And I have another conference
    And I have a conference for tomorrow
    And I have a get conferences for today request with a valid date
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And a list containing only todays hearings conference details should be retrieved

  Scenario: Get conference details by hearing id
    Given I have a conference
    And I have a get details for a conference request by hearing id with a valid username
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the conference details should be retrieved

  Scenario: Get expired conferences
    Given I have a conference
    And I have another conference
    And I close the last created conference
    And I have a get expired conferences request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And I have an empty list of expired conferences

  Scenario: Close all conferences
    Given I have a conference
    And I have another conference
    And I have a conference for tomorrow
    And I close all conferences
    And I have a get expired conferences request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And a list not containing the closed hearings should be retrieved
