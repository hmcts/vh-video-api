Feature: Callbacks
  In order to keep VH data up to date
  As an API service
  I want to handle external events

  Scenario: Fail to send an event request for non-existent conference
    Given I have a nonexistent conference event request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Fail to send an event request for non-existent participant in conference
    Given I have a room transfer event request for a nonexistent participant
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False
  
  Scenario: Fail to send an event invalid request
    Given I have an invalid conference event request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'ConferenceId is required'
    And the error response message should also contain 'ConferenceId format is not recognised'
    And the error response message should also contain 'EventId is required'
    And the error response message should also contain 'EventType is required'

  Scenario: Transfer event for judge into consultation room
    Given I have a conference
    And I have a judge consultation room
    And I have a transfer judge into consultation room event
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the judge should be in the consultation room

  Scenario: Transfer event for judge out of consultation room
    Given I have a conference
    And I have a judge consultation room
    And the judge is in the consultation room
    And I have a transfer judge out of a consultation room event
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the judge should be in the waiting room
    
  Scenario Outline: Should accept and process a conference event request
    Given I have a conference
    And I have a participant consultation room
    And I have a valid conference event request for event type <EventType>
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
	Examples: 
    | EventType              |
    | Joined                 |
    | Disconnected           |
    | Transfer               |
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

  Scenario Outline: Should accept and process a endpoint event request
    Given I have a conference with endpoints
    And I have a valid conference event request for event type <EventType>
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the endpoint status should be <EndpointStatus>
    Examples:
      | EventType             | EndpointStatus  |
      | EndpointJoined        | Connected       |
      | EndpointDisconnected  | Disconnected    |
      | EndpointTransfer      | InConsultation  |

  Scenario Outline: Should accept and process an old endpoint event request but not update the status if timestamp is old
    Given I have a conference with endpoints
    And I have a valid conference event request for event type <EventType1>
    When I send the request to the endpoint
    And I have a valid conference event request for event type <EventType2>
    When I send the request to the endpoint
    And I have an old valid conference event request for event type <EventType3>
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the endpoint status should be <EndpointStatus>
    Examples:
      | EventType1      | EventType2      | EventType3            | EndpointStatus  |
      | EndpointJoined  | EndpointJoined  | EndpointDisconnected  | Connected       |

   Scenario Outline: Should accept and process a phone event request
    Given I have a conference
    And I have a participant consultation room
    And I have a valid conference phone event request for event type <EventType>
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
  	Examples: 
    | EventType              |
    | Joined                 |
    | Disconnected           |
    | Transfer               |
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

  Scenario Outline: Should accept and process an event request with a participant room id
    Given I have a conference
    And I have a civilian interpreter room
    And I have a valid conference event request with a room id for event type <EventType>
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the room count should differ by <RoomCount>
    Examples:
      | EventType              |RoomCount |
      | RoomParticipantJoined  | 1        |

  Scenario Outline: Should accept and process an event request with an existing participant room id
    Given I have a conference
    And I have a participant consultation room
    And I have a civilian interpreter room with a participant
    And I have a valid conference event request with a room id and participant id for event type <EventType>
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the room count should differ by <RoomCount>
    Examples:
      | EventType                   |RoomCount |
      | RoomParticipantDisconnected | -1       |
      | RoomParticipantTransfer     | 0        |

