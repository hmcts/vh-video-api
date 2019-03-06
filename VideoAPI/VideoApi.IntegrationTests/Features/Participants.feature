Feature: Participants
  In order to manage participants in a conference
  As an API service
  I want to create, update and retrieve participant data

  Scenario: Update status for an existing conference and existing participant id
    Given I have an update participant status request for a valid conference
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

  Scenario: Update status for an invalid conference and existing participant id
    Given I have an update participant status request for an invalid conference
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'Please provide a valid conferenceId'
    
  Scenario: Update status for an nonexistent conference and existing participant id
    Given I have an update participant status request for a nonexistent conference
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Update status for an existing conference and nonexistent participant id
    Given I have an update participant status request for a nonexistent participant
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False
    
  Scenario: Update status for an existing conference, existing participant id and invalid request body
    Given I have an update participant status request for an negative participant
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'Invalid state provided'

    
  Scenario: Update status for an existing conference, invalid participant id
    Given I have an update participant status request for a invalid participant
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'Please provide a valid participantId'