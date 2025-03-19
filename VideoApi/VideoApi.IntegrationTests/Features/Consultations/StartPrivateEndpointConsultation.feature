Feature: Start Private Endpoint Consultation
  In order to manage private consultations between Endpoints and Defence Advocates
  As an API service
  I want to start private consultations with endpoints on behalf of defence advocates
    
  @Ignore  
  Scenario: Successfully start a private consultation with an endpoint
    Given I have a conference with endpoints
    And I have a start endpoint consultation
    When I send the request to the endpoint
    Then the response should have the status OK and success status True

  Scenario: Fail to start a private consultation for non-existent conference
    Given I have a conference with endpoints
    And I have a start endpoint consultation for a non-existent conference
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Fail to start a private consultation for non-existent endpoint
    Given I have a conference with endpoints
    And I have a start endpoint consultation for a non-existent endpoint
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False
