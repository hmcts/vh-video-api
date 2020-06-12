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

  @VIH-5598
  Scenario: Get the heartbeat data for a participant
    Given I have a conference
    And I have a participant with heartbeat data
    And I have a valid get heartbeat data request 
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the heartbeat data is retrieved
  
  @VIH-5599
  Scenario: Set the heartbeat data for a participant
    Given I have a conference
    And I have a valid set heartbeat data request 
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

  @VIH-6002
  Scenario: Get the a distinct list of judge names
    Given I have multiple conferences with duplicate first names for judges
    And I have a valid get judge names data request 
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the judge names should be retrieved
