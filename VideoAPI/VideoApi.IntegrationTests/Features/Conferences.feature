Feature: Conferences
  In order to manage conferences
  As an API service
  I want to create, update and retrieve conference data
  
  
  Scenario: Create a new conference
    Given I have a valid book a new conference request
    When I send the request to the endpoint
    Then the response should have the status Created and success status True
    And the conference details should be retrieved