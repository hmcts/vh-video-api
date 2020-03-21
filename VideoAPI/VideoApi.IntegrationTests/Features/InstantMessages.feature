Feature: Instant Messages
  In order to facilitate chat functionality
  As an API service
  I want to save message and retrieve messages

  Scenario: Get instant messages
    Given I have a conference
    And the conference has messages
    And I have a valid get instant messages request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the chat message should be retrieved

  Scenario: Get instant messages with a nonexistent conference
    Given I have a nonexistent get instant messages request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Set instant messages
    Given I have a conference
    And I have a valid set instant message request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True

  Scenario: Set instant messages for a nonexistent conference
    Given I have a nonexistent set instant message request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Delete instant messages
    Given I have a conference
    And the conference has messages
    Given I have a valid delete messages from a conference request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the messages have been deleted

  Scenario: Delete instant messages for a non-existent conference
    Given I have a nonexistent delete messages from a conference request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Delete instant messages for an invalid conference request
    Given I have an invalid delete messages from a conference request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'Please provide a valid conferenceId'
    
  Scenario: Get closed conference messages over 30 mins old 
    Given I have several closed conferences with messages
    And I have a valid get closed conferences with messages request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the responses list should contain closed conferences

  Scenario: Get closed conference messages over 30 mins old does not retreive with no IMs 
    Given I have a many closed conferences with no messages
    And I have a valid get closed conferences with messages request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the response returns an empty list without messages

  Scenario: Get open conferences with IMs
    Given I have a many open conferences with messages
    And I have a valid get closed conferences with messages request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the response returns an empty list without messages
