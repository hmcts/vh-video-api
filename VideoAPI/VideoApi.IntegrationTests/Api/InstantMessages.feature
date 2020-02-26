Feature: Instant Messages
  In order to facilitate chat functionality
  As an API service
  I want to save message and retrieve messages

  Scenario: Get instant message history for a conference
    Given I have a valid conference with messages
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the chat messages should be retrieved

  Scenario: Attempt to get instant message history for a non-existence conference
    Given I have a Nonexistent conference with messages
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Add a new instant message to a conference
    Given I have a valid conference with valid participants save message request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True

  Scenario: Attempt to add instant message to a conference from a participant who does not exist
    Given I have a valid conference with Nonexistent participants save message request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

Scenario: Get closed conferences with IMs
    Given I have a many closed conferences with messages
    And I send the request to the get closed conferences endpoint
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the responses list should contain closed conferences