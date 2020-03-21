Feature: Respond to Admin Consultations
  In order to manage private consultations with the VH Admin team
  As an API service
  I want to respond to private consultations with the VH Admin team

  Scenario: Respond to a VH Officer Consultation with an invalid request
    Given I have a conference
    And I have an invalid respond to admin consultation request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'ConferenceId is required'
    And the error response message should also contain 'ParticipantId is required'
    And the error response message should also contain 'Room must be a consultation room'
    And the error response message should also contain 'Answer to request is required'

  Scenario: Respond to a VH Officer Consultation for a non-existent conference
    Given I have a conference
    Given I have a nonexistent respond to admin consultation request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Respond to a VH Officer Consultation for a non-existent participant
    Given I have a conference
    Given I have a respond to admin consultation request with a non-existent participant
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Respond to a VH Officer Consultation successfully
    Given I have a conference
    And I have a valid respond to admin consultation request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
