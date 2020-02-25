Feature: Messages
  In order to facilitate chat functionality
  As an API service
  I want to save message and retrieve messages

  Scenario: Get messages for a conference
    Given I have a valid conference with messages
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the chat messages should be retrieved

  Scenario: Get messages for a non-existence conference
    Given I have a Nonexistent conference with messages
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Successfully add a chat message
    Given I have a valid conference with valid participants save message request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True

  Scenario: Fails to add a chat message for nonexistent participants
    Given I have a valid conference with Nonexistent participants save message request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

  Scenario: Fails to add a chat message for nonexistent conference
    Given I have a Nonexistent conference with Nonexistent participants save message request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Remove messages for an existing conference
    Given I have an remove messages from a valid conference request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

  Scenario: Remove messages for an existing conference
    Given I have an remove messages from a valid conference request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

  Scenario: Remove messages for an invalid conference
    Given I have an remove messages from an invalid conference request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'Invalid conferenceId'

  Scenario: Remove messages for a non-existent conference
    Given I have an remove messages from a nonexistent conference request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

