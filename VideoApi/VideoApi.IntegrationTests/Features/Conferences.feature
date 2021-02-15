Feature: Conferences
  In order to manage conferences
  As an API service
  I want to create, update and retrieve conference data
  
  Scenario: Create a new conference
    Given I have a valid book a new conference request
    When I send the request to the endpoint
    Then the response should have the status Created and success status True
    And the conference details should be retrieved

  Scenario: Create a new conference with linked participants
    Given I have a valid book a new conference request with linked participants
    When I send the request to the endpoint
    Then the response should have the status Created and success status True
    And the conference details should be retrieved

  Scenario: Create a new conference with jvs endpoints
    Given I have a valid book a new conference request with jvs endpoints
    When I send the request to the endpoint
    Then the response should have the status Created and success status True
    And the conference details should be retrieved with jvs endpoints
    
  Scenario: Create a new conference twice
    Given I have a valid book a new conference request
    When I send the request to the endpoint
    And I save the conference details
    And I send the request to the endpoint
    Then the response should be the same

  Scenario: Fail to book a conference with invalid request
    Given I have an invalid book a new conference request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'HearingRefId is required'
    And the error response message should also contain 'CaseType is required'
    And the error response message should also contain 'CaseNumber is required'
	  And the error response message should also contain 'ScheduledDuration is required'
    And the error response message should also contain 'ScheduledDateTime cannot be in the past'
    And the error response message should also contain 'Please provide at least one participant'

  Scenario: Get details for an existing conference
    Given I have a conference with endpoints
    And I have a get details for a conference request with a valid conference id
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

  Scenario: Remove an existing conference
    Given I have a conference
    And I have a valid remove conference request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the conference should be removed

  Scenario: Remove an invalid conference
    Given I have an invalid remove conference request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'Please provide a valid conferenceId'

  Scenario: Remove an non-existent conference
    Given I have a nonexistent remove conference request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Get details for an existing conference by hearing ref id 
    Given I have a conference
    And I have a get details for a conference request with a valid hearing ref id
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the conference details should be retrieved

  Scenario: Get details for an invalid conference by hearing ref id
    Given I have a get details for a conference request with an invalid hearing ref id
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'Please provide a valid hearingRefId'

  Scenario: Get details for a non-existent conference by hearing ref id
    Given I have a get details for a conference request with a nonexistent hearing ref id
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False
  
  Scenario: Update a conference with valid request
    Given I have a conference
    And I have a valid update a conference request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True

  Scenario: Update a conference with audio recording required for a valid request
    Given I have a conference
    And I have a valid update a conference request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the conference should be updated

  Scenario: Update a conference with invalid request
    Given I have a invalid update a conference request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

  Scenario: Update a conference with non-existent request
    Given I have a nonexistent update a conference request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Get expired open conferences by scheduled date
    Given I have several conferences
    And I have a valid get expired open conferences by scheduled date request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    Then a list without closed conferences is retrieved

  Scenario: Close conference
    Given I have a conference
    And I have a valid close conference request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the conference should be closed

  Scenario: Close conference with nonexistent request
    Given I have a close conference request for a nonexistent conference id
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Close conference with invalid request
    Given I have an invalid close conference request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

Scenario: Anonymise Conference and Participant data for hearing older than 3 months
  Given I have a conference closed over 3 months ago
  And I have a request to anonymise the data
  When I send the request to the endpoint
  Then the response should have the status NoContent and success status True
  And the conference data should be anonymised

Scenario: Remove heartbeats for conferences older than 14 days
  Given I have a conference over -14 days old
  And I have a participant with heartbeat data
  And I have a request to remove heartbeats for conferences
  When I send the request to the endpoint
  Then the response should have the status NoContent and success status True
  And the heartbeats should be deleted

Scenario: Remove heartbeats for conferences within 14 days
  Given I have a conference over -10 days old
  And I have a participant with heartbeat data
  And I have a request to remove heartbeats for conferences
  When I send the request to the endpoint
  Then the response should have the status NoContent and success status True
  And the heartbeats should not be deleted
