Feature: Tasks
  In order to action specific VH events
  As an API service
  I want to handle tasks

Scenario: Get Tasks
	Given I have a conference
	And I have a valid get tasks request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
	And the task is retrieved

Scenario: Update Task
	Given I have a conference
	And I have a valid update task request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
	And the task is updated
