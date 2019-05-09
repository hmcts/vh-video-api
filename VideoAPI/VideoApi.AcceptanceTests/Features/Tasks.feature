Feature: Tasks
  In order to action specific VH events
  As an API service
  I want to handle tasks

Scenario: Get Pending Tasks
	Given I have a conference
	And The conference has a pending task
	And I have a valid get pending tasks request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
	And the pending tasks are retrieved

Scenario: Update Task
	Given I have a conference
	And The conference has a pending task
	And I have a valid get pending tasks request
	When I send the request to the endpoint
	Then the pending tasks are retrieved
	Given I have a valid update task request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
	Given I have a valid get pending tasks request
	When I send the request to the endpoint
	Then the task is updated
