Feature: Audio Recording
	In order to enable the audio recordings of hearings
  As an api service
	I want to enable CRUD processes for audio recordings

Scenario: Get Audio Recording Link - Ok
	Given I have a conference
	And the conference has an audio recording
	And I have a valid get audio recording link request
	When I send the request to the endpoint
	Then the response should have the status Ok and success status True
	And the audio recording link is retrieved

Scenario: Get Audio Recording Link - Not Found
	Given I have a conference
	And I have a valid get audio recording link request for non existing hearing
	When I send the request to the endpoint
  Then the response should have the status Ok and success status True
  And the audio recording links are empty
