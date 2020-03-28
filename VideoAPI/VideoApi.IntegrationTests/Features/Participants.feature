Feature: Participants
  In order to manage participants in a conference
  As an API service
  I want to create, update and retrieve participant data

  Scenario: Add participant
    Given I have a conference
    And I have an add participant to a valid conference request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

  Scenario: Add participant to an invalid conference
    Given I have an add participant to an invalid conference request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'Please provide a valid conferenceId'

  Scenario: Add participant to an existing conference with a bad request body
    Given I have a conference
    And I have an add participant to a conference request with an invalid body
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'Please provide at least one participant'

  Scenario: Remove participant
    Given I have a conference
    And I have an remove participant from a valid conference request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

  Scenario: Remove participant from an invalid conference
    Given I have an remove participant from an invalid conference request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'Please provide a valid conferenceId'

  Scenario: Remove participant from a nonexistent conference
    Given I have an remove participant from a nonexistent conference request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Remove nonexistent participant from an existing conference
    Given I have a conference
    And I have a remove participant from a conference request for a nonexistent participant
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Remove an invalid participant from an existing conference
    Given I have a conference
    And I have a remove participant from a conference request for a invalid participant
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'Please provide a valid participantId'

  Scenario: Get self test score using invalid identifiers
    Given I have a conference
    And I have a nonexistent get self test score request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Update participant
    Given I have a conference
    And I have an update participant to a valid conference request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

  Scenario: Update participant with nonexistent conference
    Given I have an update participant to a nonexistent conference request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Update participant with invalid conference
    Given I have an update participant to a invalid conference request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

  Scenario: Get heartbeats
    Given I have a conference
    And I have a participant with heartbeat data
    And I have a valid get heartbeats request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And 3 heartbeats should be retrieved

  Scenario: Get heartbeats with nonexistent heartbeats
    Given I have a conference
    And I have a valid get heartbeats request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And 0 heartbeats should be retrieved

  Scenario: Get heartbeats with nonexistent conference id
    Given I have a conference
    And I have a get heartbeats request with a nonexistent conference id
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False
    And the error response message should contain 'Not Found'

  Scenario: Get heartbeats with nonexistent participant id
    Given I have a conference
    And I have a get heartbeats request with a nonexistent participant id
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False
    And the error response message should contain 'Not Found'

  Scenario: Set heartbeats
    Given I have a conference
    And I have a valid set heartbeats request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the heartbeats should be saved
