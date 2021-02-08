Feature: Start Consultation
  In order to discuss judicial matters separate to the main hearing room
  As an api service
  I want to be able to join and leave consultation rooms

  @VIH-6876
  Scenario: Start Consultation for a Judge
    Given I have a booked conference
    And the judge is in the waiting room
    And I have a valid start consultation request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
