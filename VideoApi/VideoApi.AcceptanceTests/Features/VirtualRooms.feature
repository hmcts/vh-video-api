Feature: Virtual Rooms
  In order to put linked participants together
  As an API service
  I want to create create virtual meeting rooms

  Scenario: Get an interpreter room
    Given I have a conference
    And I have a get interpreter room request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the response should have connection details for the room
