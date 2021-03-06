Feature: Events
  In order to keep VH data up to date
  As an API service
  I want to handle external events

  Scenario: Add conference event
    Given I have a conference
    And I have a valid conference event request for event type Joined
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the status is updated

  Scenario: Add Judge Joined event
    Given I have a conference
    And I have a valid conference event request for a Judge with event type Joined
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the status is updated

  Scenario Outline: Add participant event
    Given I have a conference
    Given I have a valid conference event request for event type <EventType>
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

    Examples:
      | EventType              |
      | Joined                 |
      | Disconnected           |
      | Start                  |
      | CountdownFinished      |
      | Pause                  |
      | Close                  |
      | Leave                  |
      | MediaPermissionDenied  |
      | ParticipantJoining     |
      | Help                   |
      | ParticipantNotSignedIn |
      | ConnectingToEventHub   |
      | SelectingMedia         |
      | ConnectingToConference |

  Scenario Outline: Endpoint Event
    Given I have a conference
    And I have endpoints stored against a conference
    And I have a valid endpoint event request for event type <EventType>
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the endpoint status should be <EndpointStatus>

    Examples:
      | EventType            | EndpointStatus  |
      | EndpointJoined       | Connected       |
      | EndpointDisconnected | Disconnected    |
