Feature: Audio Recording
	In order to enable the audio recordings of hearings
  As an api service
	I want to enable CRUD processes for audio recordings

@VIH-5868
Scenario: Get Audio Application
    Given I have a conference
    And the conference has an audio application
    And I have a valid get audio application request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the audio application details are retrieved

@VIH-5868
Scenario: Create Audio Application
    Given I have a conference
    And I have a valid create audio application request
    When I send the request to the endpoint
    Then the response should have the status Created and success status True

@VIH-5868
Scenario: Delete Audio Application
    Given I have a conference
    And the conference has an audio application
    And I have a valid delete audio application request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

@VIH-5868
Scenario: Create Audio Application and Stream
    Given I have a conference
    And I have a valid create audio application and stream request
    When I send the request to the endpoint
    Then the response should have the status Created and success status True

@VIH-5868
Scenario: Get Audio Stream
    Given I have a conference
    And the conference has an audio stream
    And I have a valid get audio stream request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the audio stream details are retrieved

@VIH-5868
Scenario: Create Audio Stream
    Given I have a conference
    And the conference has an audio application
    And I have a valid create audio stream request
    When I send the request to the endpoint
    Then the response should have the status Created and success status True

@VIH-5868
Scenario: Delete Audio Stream
    Given I have a conference
    And I have a valid delete audio stream request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

@VIH-5868
Scenario: Get Audio Stream Monitoring
    Given I have a conference
    And the conference has an audio stream
    And I have a valid get audio stream monitoring request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the audio stream monitoring details are retrieved

@VIH-5868 @Ignore
Scenario: Get Audio Recording Link
    Given I have a conference
    And the conference has an audio recording
    And I have a valid get audio recording link request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the audio recording link is retrieved
