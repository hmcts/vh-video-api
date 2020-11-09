Feature: Tasks
  In order to track tasks to be actions
  As an API service
  I want to create new tasks and retrieve pending tasks

  Scenario: Get tasks for a conference
    Given I have a conference
    And A conference has tasks
    And I have a valid get tasks request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the list of tasks should be retrieved

  Scenario: Get tasks for a nonexistent conference
    Given I have a conference
    And A conference has tasks
    And I have a nonexistent get tasks request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True

  Scenario: Update task status
    Given I have a conference
    And A conference has tasks
    And I have a valid update task request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the task should be retrieved with updated details

  Scenario: Update task status for a task that does not exist
    Given I have a conference
    And A conference has tasks
    And I have a nonexistent update task request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Update task status for a task with invalid details
    Given I have a conference
    And A conference has tasks
    And I have an invalid update task request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

  Scenario: Add a task for a participant in a conference
    Given I have a conference
    And I have a valid add Task for a participant in a conference request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

  Scenario: Add a task for a participant in a conference with invalid details
    Given I have a conference
    And I have an invalid add Task for a participant in a conference request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
