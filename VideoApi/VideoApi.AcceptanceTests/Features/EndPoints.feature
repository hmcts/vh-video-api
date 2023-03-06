Feature: Endpoints
  In order to manage endpoints in a conference
  As an api service
  I want to get, set, update or delete endpoint data

Scenario: Add an endpoint to a valid conference 
    Given I have a conference
    And I have add endpoint to a conference request with a valid conference id
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the endpoint should be added

Scenario: Add an endpoint to an invalid conference 
    Given I have a conference
    And I have add endpoint to a conference request with a Invalid conference id
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

Scenario: Add an endpoint to an nonexistent conference 
    Given I have a conference
    And I have add endpoint to a conference request with a Nonexistent conference id
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

Scenario: Add an endpoint with invalid data to a valid conference 
    Given I have a conference
    And I have add endpoint with invalid data to a conference request with a valid conference id
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

Scenario: Remove an endpoint from a valid conference 
    Given I have a conference
    And I have endpoints stored against a conference
    And I have remove endpoint to a conference request with a valid conference id
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the endpoint should be deleted

Scenario: Remove an endpoint from an invalid conference 
    Given I have a conference
    And I have endpoints stored against a conference
    And I have remove endpoint to a conference request with a Invalid conference id
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

Scenario: Remove an endpoint from an nonexistent conference 
    Given I have a conference
    And I have endpoints stored against a conference
    And I have remove endpoint to a conference request with a Nonexistent conference id
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

Scenario: Remove an nonexistent endpoint from a valid conference 
    Given I have a conference
    And I have remove nonexistent endpoint to a conference request with a valid conference id
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

Scenario: Update an endpoint to a valid conference 
    Given I have a conference
    And I have endpoints stored against a conference
    And I have update endpoint to a conference request with a valid conference id
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the endpoint should be updated

Scenario: Update an endpoint with invalid data to a valid conference 
    Given I have a conference
    And I have endpoints stored against a conference
    And I have update endpoint with invalid data to a conference request with a valid conference id
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    
Scenario: Update an endpoint to an invalid conference 
    Given I have a conference
    And I have endpoints stored against a conference
    And I have update endpoint to a conference request with a Invalid conference id
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

Scenario: Update an endpoint to an nonexistent conference 
    Given I have a conference
    And I have endpoints stored against a conference
    And I have update endpoint to a conference request with a Nonexistent conference id
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

Scenario: Update an nonexistent endpoint from a valid conference 
    Given I have a conference
    And I have update nonexistent endpoint to a conference request with a valid conference id
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

Scenario: Get endpoints for an existing conference
  Given I have a conference
  And I have endpoints stored against a conference
  And I have a get endpoints for a endpoints request with a valid conference id
  When I send the request to the endpoint
  Then the response should have the status OK and success status True
  And the endpoints should be retrieved

