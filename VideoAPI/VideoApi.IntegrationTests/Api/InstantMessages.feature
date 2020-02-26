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

  Scenario: Fails to add a instant message for nonexistent conference
    Given I have a Nonexistent conference with Nonexistent participants save message request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Remove instant messages for an existing conference
    Given I have a remove messages from a valid conference request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

  Scenario: Remove instant messages for an invalid conference
    Given I have a remove messages from an Invalid conference request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'Please provide a valid conferenceId'

  Scenario: Remove instant messages for a non-existent conference
    Given I have a remove messages from a Nonexistent conference request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False
    
  Scenario: Get closed conferences with IMs
    Given I have a many closed conferences with messages
    And I send the request to the get closed conferences endpoint
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the responses list should contain closed conferences

  Scenario: Get closed conferences with no IMs 
    Given I have a many closed conferences with no messages
    And I send the request to the get closed conferences endpoint
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False
    And the response is an empty list should

  Scenario: Get open conferences with IMs
    Given I have a many open conferences with messages
    And I send the request to the get closed conferences endpoint
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False
    And the response is an empty list should    

