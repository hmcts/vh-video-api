Feature: Leave Private Consultations
  In order to manage private consultations
  As an API service
  I want to leave private consultations

  Scenario: Leave a private consultation with an invalid request
    Given I have an invalid leave consultation request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'ConferenceId is required'
    And the error response message should also contain 'ParticipantId is required'

  Scenario: Successfully leave a private consultation
    Given I have a conference
    And I have an valid leave consultation request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

  Scenario: Leaving a consultation for a non-existent conference returns not found
    Given I have a conference
    And I have an nonexistent leave consultation request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Leaving a consultation for a non-existent participant returns not found
    Given I have a conference
    And I have a leave consultation request for a nonexistent participant
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Leaving a consultation for a participant not in a consultation returns a bad request
    Given I have a conference
    And I have a leave consultation request for a participant not in a consultation
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'is not in a consultation room'