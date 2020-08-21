Feature: Endpoints
  In order to manage endpoints in a conference
  As an API service
  I want to create, update and retrieve endpoint data
  
  Scenario: Get list of endpoints for non-existent conference
    Given I have a get endpoints request for a nonexistent conference
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the endpoint response should be empty

  Scenario: Get an empty list of endpoints for a conference
    Given I have a conference with no endpoints
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the endpoint response should be empty

  Scenario: Get a list of endpoints for a conference
    Given I have a conference with endpoints
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the endpoint response should not be empty



Scenario: Update non-existent endpoint
    Given I have a conference with no endpoints
    And I have update to a non-existent endpoint for a conference request
    When I send the request to the endpoint
    Then the response should have the status InternalServerError and success status False

Scenario: Update an endpoint from a conference
    Given I have a conference with endpoints
    And I have update endpoint for a conference request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
