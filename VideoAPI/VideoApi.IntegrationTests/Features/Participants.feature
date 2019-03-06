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

  Scenario: Add participant to an existing conference
    Given I have an add participant to a valid conference request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

  Scenario: Add participant to a non-existent conference
    Given I have an add participant to a nonexistent conference request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Add participant to an invalid conference
    Given I have an add participant to an invalid conference request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'Please provide a valid conferenceId'

  Scenario: Add participant to an existing conference with a bad request body
    Given I have an add participant to a conference request with an invalid body
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'Please provide at least one participant'

  Scenario: Remove participant from an existing conference
    Given I have an remove participant from a valid conference request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

  Scenario: Remove participant from an invalid conference
    Given I have an remove participant from an invalid conference request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'Please provide a valid conferenceId'


  Scenario: Remove participant from a non-existent conference
    Given I have an remove participant from a nonexistent conference request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Remove non-existent participant from an existing conference
    Given I have a remove participant from a conference request for a nonexistent participant
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Remove an invalid participant from an existing conference
    Given I have a remove participant from a conference request for a invalid participant
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'Please provide a valid participantId'
