Feature: Respond To Consultations
  In order to manage private consultations
  As an API service
  I want to respond to private consultations

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