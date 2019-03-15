Feature: Conferences
  In order to manage conferences
  As an API service
  I want to create, update and retrieve conference data

  Scenario: Get conference details by username
	Given I have a get details for a conference request by username with a valid username
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the summary of conference details should be retrieved

  Scenario: Conference details not retrieved with a non-existent username
	Given I have a get details for a conference request by username with a nonexistent username
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Create a new conference
    Given I have a valid book a new conference request
    When I send the request to the endpoint
    Then the response should have the status Created and success status True
    And the conference details should be retrieved

  Scenario: Fail to book a conference with invalid request
    Given I have an invalid book a new conference request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'HearingRefId is required'
    And the error response message should also contain 'CaseType is required'
    And the error response message should also contain 'CaseNumber is required'
    And the error response message should also contain 'ScheduledDateTime cannot be in the past'
    And the error response message should also contain 'Please provide at least one participant'

  Scenario: Get details for an existing conference
    Given I have a get details for a conference request with a valid conference id
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the conference details should be retrieved

  Scenario: Get details for an invalid conference
    Given I have a get details for a conference request with an invalid conference id
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'Please provide a valid conferenceId'

  Scenario: Get details for a non-existent conference
    Given I have a get details for a conference request with a nonexistent conference id
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Update status for an existing conference with a valid request
    Given I have a valid update conference status request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

  Scenario: Update status for an invalid conference a valid request
    Given I have an invalid update conference status request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'Please provide a valid conferenceId'

  Scenario: Update status for an existing conference with an invalid request
    Given I have a invalid update conference status request for an existing conference
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'Invalid state provided'

  Scenario: Update status for a non-existent conference
    Given I have a nonexistent update conference status request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Remove an existing conference
    Given I have a valid remove conference request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the conference should be removed

  Scenario: Remove an invalid conference
    Given I have an invalid remove conference request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'Please provide a valid conferenceId'

  Scenario: Remove an non-existant conference
    Given I have a nonexistent remove conference request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False