Feature: Callbacks
  In order to keep VH data up to date
  As an API service
  I want to handle external events

  Scenario: Fail to send an event request for non-existent conference
    Given I have a nonexistent conference event request
    When I send the request to the endpoint
    Then the response should have the status InternalServerError and success status False

  Scenario: Fail to send an event request for non-existent participant in conference
    Given I have a room transfer event request for a nonexistent participant
    When I send the request to the endpoint
    Then the response should have the status InternalServerError and success status False
  
  Scenario: Fail to send an event invalid request
    Given I have an invalid conference event request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'ConferenceId is required'
    And the error response message should also contain 'ConferenceId format is not recognised'
    And the error response message should also contain 'EventId is required'
    And the error response message should also contain 'EventType is required'

  Scenario Outline: Should accept and process a conference event request
    Given I have a conference
    And I have a valid conference event request for event type <EventType>
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
	Examples: 
    | EventType              |
    | Joined                 |
    | Disconnected           |
    | Transfer               |
    | Pause                  |
    | Close                  |
    | Leave                  |
    | JudgeAvailable         |
    | JudgeUnavailable       |
    | MediaPermissionDenied  |
    | ParticipantJoining     |
    | Help                   |
    | ParticipantNotSignedIn |
