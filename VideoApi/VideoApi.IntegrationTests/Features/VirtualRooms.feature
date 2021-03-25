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

Scenario: Get an interpreter room for non-existent conference
  Given I have a conference
  And I have a get interpreter room request for a non-existent conference
  When I send the request to the endpoint
  Then the response should have the status NotFound and success status False

Scenario: Get an interpreter room for non-existent participant
  Given I have a conference
  And I have a get interpreter room request for a non-existent participant
  When I send the request to the endpoint
  Then the response should have the status NotFound and success status False

  Scenario: Get an witness room
    Given I have a conference
    And I have a get witness room request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the response should have connection details for the room

  Scenario: Get an witness room for non-existent conference
    Given I have a conference
    And I have a get witness room request for a non-existent conference
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Get an witness room for non-existent participant
    Given I have a conference
    And I have a get witness room request for a non-existent participant
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Get a judicial office holder room
    Given I have a conference
    And I have a get judicial office holder room request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the response should have connection details for the room
