Feature: Audio Recording
	In order to enable the audio recordings of hearings
  As an api service
	I want to enable CRUD processes for audio recordings

@VIH-5868
Scenario: Get Audio Application - OK
    Given I have a conference
    And the conference has an audio application
    And I have a valid get audio application request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the audio application details are retrieved

@VIH-5868
Scenario: Create Audio Application - Created
    Given I have a conference
    And I have a valid create audio application request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True

@VIH-5868
Scenario: Delete Audio Application - OK
    Given I have a conference
    And the conference has an audio application
    And I have a valid delete audio application request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

@VIH-5868
Scenario: Create Audio Application and Stream - Created
    Given I have a conference
    And I have a valid create audio application and stream request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True

@VIH-5868
Scenario: Get Audio Stream - OK
    Given I have a conference
    And the conference has an application and an audio stream
    And I have a valid get audio stream request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the audio stream details are retrieved

@VIH-5868
Scenario: Create Audio Stream - Created
    Given I have a conference
    And the conference has an audio application
    And I have a valid create audio stream request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True

@VIH-5868
Scenario: Delete Audio Stream - OK
    Given I have a conference
    And the conference has an audio stream
    And I have a valid delete audio stream request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

@VIH-5868
Scenario: Get Audio Monitoring Stream - OK
    Given I have a conference
    And the conference has an application and an audio stream
    And I have a valid get audio monitoring stream request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the audio monitoring stream details are retrieved

@VIH-5868
Scenario: Get Audio Recording Link - OK
    Given I have a conference
    And I have an audio recording
    And I have a valid get audio recording link request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the audio recording link details are retrieved

@VIH-5868
Scenario: Get Audio Recording Link - Not Found
    Given I have a nonexistent get audio recording link request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False
