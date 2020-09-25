Feature: Audio Recording
	In order to enable the audio recordings of hearings
  As an api service
	I want to enable CRUD processes for audio recordings

@VIH-5868
Scenario: Get Audio Application - OK
  Given I have a conference
	And I have a valid get audio application request
	When I send the request to the endpoint
	Then the response should have the status Ok and success status True
	And the audio application details are retrieved

@VIH-5868
Scenario: Get Audio Application - Not Found
  Given I have a nonexistent get audio application request
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

@VIH-5868
Scenario: Create Audio Application - Conflict
  Given I have a conference
  And I have a valid create audio application request
	When I send the request to the endpoint
	Then the response should have the status Conflict and success status False

@VIH-5868
Scenario: Delete Audio Application with audio recording file - No Content
	Given I have a conference with an audio application and audio recording file
	And I have a valid delete audio application request
	When I send the request to the endpoint
	Then the response should have the status NoContent and success status True

@VIH-5868
Scenario: Delete Audio Application - Not Found
  Given I have a valid delete audio application request that has no application
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

  @VIH-5868
Scenario: Delete Audio Application without audio recording file - Not Found
  Given I have a conference
	And I have a valid non-existent delete audio application request
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

@VIH-5868
Scenario: Get Audio Stream - Not Found
	Given I have a valid get audio stream request that has no stream
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

@VIH-5868
Scenario: Get Audio Stream Monitoring - Ok
  Given I have a conference with audiorecording
	And I have a valid get audio stream monitoring request
	When I send the request to the endpoint
	Then the response should have the status Ok and success status True
	And the audio stream monitoring details are retrieved

@VIH-5868
Scenario: Get Audio Stream Monitoring - Not Found
	Given I have a valid get audio stream monitoring request that has no audio stream
	When I send the request to the endpoint
	Then the response should have the status NotFound and success status False

@VIH-5868
Scenario: Get Audio Recording Link - Ok
	Given I have a conference
	And the conference has an audio recording
	And I have a valid get audio recording link request
	When I send the request to the endpoint
	Then the response should have the status Ok and success status True
	And the audio recording link is retrieved

@VIH-5868
Scenario: Get Audio Recording Link - Not Found
	Given I have a conference
	And I have a valid get audio recording link request for non existing hearing
	When I send the request to the endpoint
  Then the response should have the status Ok and success status True
  And the audio recording links are empty
