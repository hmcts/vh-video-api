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
    And I have an audio recording
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

  @VIH-6385
  Scenario Outline: Get Audio Recordings for CVP With case reference number
    Given Cvp has audio recordings
      | CloudRoomName  | Date       | CaseReference |
      | TestCloudRoom1 | 2020-01-01 | MyReference1  |
      | TestCloudRoom1 | 2020-01-01 | MyReference1  |
      | TestCloudRoom1 | 2020-01-02 | MyReference3  |
      | TestCloudRoom1 | 2020-01-02 | MyReference4  |
      | TestCloudRoom1 | 2020-01-02 | MyReference5  |
    And I have a valid get cvp audio recordings request for <CloudRoomName> <Date> <CaseReference>
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And <Results> audio recordings from cvp are retrieved
    Examples:
      | CloudRoomName  | Date       | CaseReference         | Results |
      | TestCloudRoom1 | 2020-01-01 | MyReference1          | 2       |
      | TestCloudRoom1 | 2020-01-02 | MyReference3          | 1       |
      | TestCloudRoom1 | 2020-01-02 | MyReference4          | 1       |
      | TestCloudRoom1 | 2020-01-02 | MyReference5          | 1       |
      | TestCloudRoom1 | 2020-01-02 | MyReference99999xxxxx | 0       |
      | NoExist        | 2020-01-09 | NoExist               | 0       |

  @VIH-6385
  Scenario Outline: Get Audio Recordings for CVP without case reference number
    Given Cvp has audio recordings
      | CloudRoomName  | Date       |
      | TestCloudRoom1 | 2020-01-01 |
      | TestCloudRoom1 | 2020-01-01 |
      | TestCloudRoom1 | 2020-01-02 |
      | TestCloudRoom1 | 2020-01-02 |
      | TestCloudRoom1 | 2020-01-02 |
      | TestCloudRoom1 | 2020-01-03 |
    And I have a valid get cvp audio recordings request for <CloudRoomName> <Date>
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And <Results> audio recordings from cvp are retrieved
    Examples:
      | CloudRoomName  | Date       |  Results |
      | TestCloudRoom1 | 2020-01-01 |  2       |
      | TestCloudRoom1 | 2020-01-02 |  3       |
      | TestCloudRoom1 | 2020-01-03 |  1       |
      | TestCloudRoom1 | 2020-01-04 |  0       |
