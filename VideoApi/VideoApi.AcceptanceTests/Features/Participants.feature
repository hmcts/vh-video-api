Feature: Participants
  In order to manage participants in a conference
  As an API service
  I want to create, update and retrieve participant data

  Scenario: Add participant
    Given I have a conference
    And I have an add participant to a valid conference request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the participant is added

  Scenario: Remove participant
    Given I have a conference
    And I have an remove participant from a valid conference request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the participant is removed

  Scenario: Update participant
    Given I have a conference
    And I have an update participant details request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the participant is updated

  Scenario: Add linked participants
    Given I have a conference
    And I have a request to add two linked participants
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the participant is added
    And the linked participants are added
