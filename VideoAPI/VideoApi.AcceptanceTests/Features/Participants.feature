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

Scenario: Get test score result
    Given I have a conference
	And the participant has a self test score
	And I have a get test score result request
	When I send the request to the endpoint
	Then the response should have the status NoContent and success status True
	And the score should be good

Scenario: Get independent test score result
	Given I have a conference
	And the participant has a self test score
	And I have a get independent test score result request
	When I send the request to the endpoint
	Then the response should have the status NoContent and success status True
	And the score should be good

Scenario: Update self test score result
	Given I have a conference
	And I have an update self test score result request
	When I send the request to the endpoint
	Then the response should have the status NoContent and success status True