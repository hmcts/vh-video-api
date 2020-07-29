Feature: Conference Management
  In order to manage video hearings
  As an API service
  I want to start, stop and pause hearings

  Scenario: Start a video hearing
    Given I have a conference
    And I have a start video hearing request
    When I send the request to the endpoint
    Then the response should have the status Accepted and success status True

  Scenario: Pause a video hearing
    Given I have a conference
    And I have a pause video hearing request
    When I send the request to the endpoint
    Then the response should have the status Accepted and success status True

  Scenario: End a video hearing
    Given I have a conference
    And I have a end video hearing request
    When I send the request to the endpoint
    Then the response should have the status Accepted and success status True
