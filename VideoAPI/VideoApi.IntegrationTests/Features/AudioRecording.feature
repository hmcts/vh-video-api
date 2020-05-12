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
Scenario: Get Audio Application - Not Found
    Given I have a nonexistent get audio application request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

@VIH-5868
Scenario: Create Audio Application - Created
    Given I have a conference
    And I have a valid create audio application request
    When I send the request to the endpoint
    Then the response should have the status Created and success status True

@VIH-5868
Scenario: Create Audio Application - Not Found
    And I have a nonexistent create audio application request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

@VIH-5868
Scenario: Create Audio Application - Conflict
    Given I have a conference
    And I have a valid create audio application request
    When I send the request to the endpoint
    Then the response should have the status Created and success status True
    When I have a duplicate create audio application request
    Then the response should have the status Conflict and success status False

@VIH-5868
Scenario: Delete Audio Application - OK
    Given I have a conference
    And the conference has an audio application
    And I have a valid delete audio application request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

@VIH-5868
Scenario: Delete Audio Application - Not Found
    Given I have a nonexistent delete audio application request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

@VIH-5868
Scenario: Create Audio Application and Stream - Created
    Given I have a conference
    And I have a valid create audio application and stream request
    When I send the request to the endpoint
    Then the response should have the status Created and success status True

@VIH-5868
Scenario: Create Audio Application and Stream - Not Found
    Given I have a conference
    And I have a nonexistent create audio application and stream request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

@VIH-5868
Scenario: Create Audio Application and Stream - Conflict
    Given I have a conference
    And I have a valid create audio application and stream request
    When I send the request to the endpoint
    Then the response should have the status Created and success status True
    When I have a duplicate create audio application and stream request
    And I send the request to the endpoint
    Then the response should have the status Conflict and success status False

@VIH-5868
Scenario: Get Audio Stream - OK
    Given I have a conference
    And the conference has an application and an audio stream
    And I have a valid get audio stream request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the audio stream details are retrieved

@VIH-5868
Scenario: Get Audio Stream - Not Found
    Given I have a nonexistent get audio steam request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

@VIH-5868
Scenario: Create Audio Stream - Created
    Given I have a conference
    And the conference has an audio application
    And I have a valid create audio stream request
    When I send the request to the endpoint
    Then the response should have the status Created and success status True

@VIH-5868
Scenario: Create Audio Stream - Client Error
    Given I have a conference
    And I have a valid create audio stream request
    When I send the request to the endpoint
    Then the response should have the status ClientError and success status False

@VIH-5868
Scenario: Create Audio Stream - Not Found
    Given I have a conference
    And I have a nonexistent create audio stream request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

@VIH-5868
Scenario: Create Audio Stream - Conflict
    Given I have a conference
    And the conference has an audio application
    And I have a valid create audio stream request
    When I send the request to the endpoint
    Then the response should have the status Created and success status True
    When I have a duplicate create audio stream request
    And I send the request to the endpoint
    Then the response should have the status Conflict and success status False

@VIH-5868
Scenario: Delete Audio Stream - OK
    Given I have a conference
    And the conference has an audio stream
    And I have a valid delete audio stream request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

@VIH-5868
Scenario: Delete Audio Stream - Not Found
    Given I have a nonexistent delete audio stream request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

@VIH-5868
Scenario: Get Audio Monitoring Stream - OK
    Given I have a conference
    And the conference has an application and an audio stream
    And I have a valid get audio monitoring stream request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the audio monitoring stream details are retrieved

@VIH-5868
Scenario: Get Audio Monitoring Stream - Not Found
    Given I have a nonexistent get audio monitoring steam request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

@VIH-5868
Scenario: Get Audio Recording Link - OK
    Given I have a conference
    And I have an audio recording
    And I have a valid get audio recording link request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the audio recording link details are retrieved

@VIH-5868
Scenario: Get Audio Monitoring Stream - Not Found
    Given I have a nonexistent get audio recording link request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False
