Feature: heatbeat
  In order to facilitate heatbeats functionality
  As an API service
  I want to save heatbeats and retrieve heatbeats

  Scenario: Get heatbeats
    Given I have a conference
    And I have heartbeats
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And 3 heartbeats should be retrieved

