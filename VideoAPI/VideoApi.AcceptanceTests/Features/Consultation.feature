Feature: Consultation
  In order to discuss judicial matters separate to the main hearing room
  As an Api Service
  I want to be able to join and leave consultation rooms

  Scenario: Start Consultation for a Judge
    Given I have a valid consultation request as a Judge
    When I send the request to the endpoint
    Then the response should have the status Accepted and success status True
