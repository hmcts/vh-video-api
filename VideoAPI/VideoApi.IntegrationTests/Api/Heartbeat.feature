Feature: heatbeat
  In order to facilitate heatbeats functionality
  As an API service
  I want to save heatbeats and retrieve heatbeats

  Scenario: Get heatbeats
    Given I have a conference
    And I have heartbeats
    Then 3 heartbeats should be retrieved

  Scenario: Save heatbeats
    Given I have a conference
    And I want to save a heartbeat
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And 1 heartbeat should be retrieved

