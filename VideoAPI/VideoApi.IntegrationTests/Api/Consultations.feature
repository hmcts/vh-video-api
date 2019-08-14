Feature: Consultations
  In order to manage private consultations
  As an API service
  I want to raise and respond to private consultations

  Scenario: Successfully raise a private consultation request
    Given I have a conference
    And I have a valid raise consultation request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    
  Scenario: Raise private consultation request against non-existent conference
    Given I have a conference
    And I have a nonexistent raise consultation request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False
  
  Scenario: Raise a private consultation request that fails validation
    Given I have an invalid raise consultation request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'RequestedBy is required'
    And the error response message should also contain 'RequestedFor is required'
    And the error response message should also contain 'ConferenceId is required'

  Scenario: Raise private consultation request when user requested by does not exist
    Given I have a conference
    And I have a raise consultation request with an invalid requestedBy
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Raise private consultation request when user requested for does not exist
    Given I have a conference
    And I have a raise consultation request with an invalid requestedFor
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

    
    ### Respond to consultation

  Scenario: Successfully respond to a private consultation request
    Given I have a conference
    And I have a valid respond consultation request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

  Scenario: Respond to private consultation request against non-existent conference
    Given I have a conference
    And I have a nonexistent respond consultation request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Respond to a private consultation request that fails validation
    Given I have an invalid respond consultation request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False
    And the error response message should also contain 'RequestedBy is required'
    And the error response message should also contain 'RequestedFor is required'
    And the error response message should also contain 'ConferenceId is required'
    And the error response message should also contain 'Answer to request is required'

  Scenario: Respond to private consultation request when user requested by does not exist
    Given I have a conference
    And I have a respond consultation request with an invalid requestedBy
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Respond to private consultation request when user requested for does not exist
    Given I have a conference
    And I have a respond consultation request with an invalid requestedFor
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False
    
    ### Leave a consultation
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