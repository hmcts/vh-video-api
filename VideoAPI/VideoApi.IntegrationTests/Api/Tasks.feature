Feature: Tasks
  In order to track tasks to be actions
  As an API service
  I want to create new tasks and retrieve pending tasks

  Scenario: Successfully add an task to a conference
    Given I have a valid add task to conference request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

  Scenario: Unable to add an task to a conference that does not exist
    Given I have a nonexistent add task to conference request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Unable to add an task to a conference with invalid details
    Given I have an invalid add task to conference request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

  Scenario: Get pending tasks for a conference
    Given I have a valid get pending tasks request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the list of tasks should be retrieved

  Scenario: Unable to get tasks for a conference that does not exist
    Given I have a nonexistent get pending tasks request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Sucessfully update task status
    Given I have a valid update task request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

  Scenario: Unable to update status of an task 
    Given I have a nonexistent update task request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Unable to update status of an task with invalid details
    Given I have an invalid update task request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
