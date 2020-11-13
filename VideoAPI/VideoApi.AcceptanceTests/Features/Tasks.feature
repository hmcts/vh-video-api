Feature: Tasks
  In order to action specific VH events
  As an API service
  I want to handle tasks

  Scenario: Get Tasks
    Given I have a conference
    And The conference has a pending task
    And I have a valid get tasks request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the tasks are retrieved

  Scenario: Update Task
    Given I have a conference
    And The conference has a pending task
    And I have a valid get tasks request
    When I send the request to the endpoint
    Then the tasks are retrieved
    Given I have a valid update task request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the task is updated
    Given I have a valid get tasks request
    When I send the request to the endpoint
    Then the tasks are retrieved

  Scenario: Add a Task to a valid conference
    Given I have a conference
    And I have add task to a conference request with a valid conference id
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the task should be added

  Scenario: Add a Task to an invalid conference
    Given I have a conference
    And I have add task to a conference request with a invalid conference id
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
