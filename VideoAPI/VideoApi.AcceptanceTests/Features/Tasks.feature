Feature: Tasks
	In order to manage tasks
	As an api service
	I want to be able to create and retrieve tasks

Scenario: Get Tasks
    Given I have a conference
	And I have a get details for a conference request by username with a valid username
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the summary of conference details should be retrieved

Scenario: Create Task
    Given I have a valid create task request
    When I send the request to the endpoint
    Then the response should have the status Created and success status True
    And the conference details should be retrieved