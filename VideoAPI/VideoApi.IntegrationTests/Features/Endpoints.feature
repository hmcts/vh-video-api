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
    And I have get endpoints for conference request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the endpoint response should be empty

  Scenario: Get a list of endpoints for a conference
    Given I have a conference with endpoints
    And I have get endpoints for conference request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the endpoint response should not be empty

  Scenario: Add an endpoint to an existing conference
    Given I have a conference with no endpoints
    And I have an add endpoint to conference request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    Given I have get endpoints for conference request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the endpoint response should be 1

  Scenario: Failed request to add an endpoint
    Given I have an invalid add endpoint to conference request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'DisplayName is required'
    And the error response message should also contain 'Pin is required'
    And the error response message should also contain 'SipAddress is required'

  Scenario: Add endpoint to non-existent conference
    Given I have an add endpoint to a non-existent conference request
    When I send the request to the endpoint
    Then the response should have the status InternalServerError and success status False
    
  Scenario: Remove non-existent endpoint
    Given I have a conference with no endpoints
    And I have remove non-existent endpoint from a conference request
    When I send the request to the endpoint
    Then the response should have the status InternalServerError and success status False
    
  Scenario: Remove an endpoint from a conference
    Given I have a conference with endpoints
    And I have remove endpoint from a conference request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
