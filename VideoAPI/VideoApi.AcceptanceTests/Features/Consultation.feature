Feature: Consultation
  In order to action specific VH events
  As an API service
  I want to handle private consultations

Scenario: Request Private Consultation
	Given I have a conference
	And I have a valid request private consultation request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True