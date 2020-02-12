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
