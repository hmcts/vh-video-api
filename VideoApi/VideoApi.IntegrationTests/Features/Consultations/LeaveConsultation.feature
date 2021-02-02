Feature: Leave Private Consultation
  In order to manage private consultations
  As an API service
  I want to leave a consultation

Scenario: Leave Consultation for a Judge Joh
	Given I have a booked conference
	And the judge joh is in the consultation room
	And I have a valid leave consultation request
	When I send the request to the endpoint
	Then the response should have the status Ok and success status True
