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
  Scenario: Get Audio Recording Link - OK
    Given I have a conference
    And I have an audio recording
    And I have a valid get audio recording link request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the audio recording link details are retrieved

  @VIH-5868 @VIH-6232
  Scenario: Get Audio Recording Link - Empty list
    Given I have a nonexistent get audio recording link request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the audio recording link details are empty

  @VIH-6385
  Scenario Outline: Get Audio Recordings for CVP by all
    Given Cvp has audio recordings
      | CloudRoom | Date       | CaseReference |
      | 1001      | 2020-01-01 | MyReference1  |
      | 1001      | 2020-01-01 | MyReference1  |
      | 1001      | 2020-01-02 | MyReference3  |
      | 1001      | 2020-01-02 | MyReference4  |
      | 1001      | 2020-01-02 | MyReference5  |
    And I have a valid get cvp audio recordings by all request for <CloudRoom> <Date> <CaseReference>
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And <Results> audio recordings from cvp are retrieved
    Examples:
      | CloudRoom | Date       | CaseReference         | Results |
      | 1001      | 2020-01-01 | MyReference1          | 2       |
      | 1001      | 2020-01-02 | MyReference3          | 1       |
      | 1001      | 2020-01-02 | MyReference4          | 1       |
      | 1001      | 2020-01-02 | MyReference5          | 1       |
      | 1001      | 2020-01-02 | MyReference99999xxxxx | 0       |
      | NoExist   | 2020-01-09 | NoExist               | 0       |

  @VIH-6385
  Scenario Outline: Get Audio Recordings for CVP by cloud room and date
    Given Cvp has audio recordings
      | CloudRoom | Date       |
      | 1001      | 2020-01-01 |
      | 1001      | 2020-01-01 |
      | 1001      | 2020-01-02 |
      | 1001      | 2020-01-02 |
      | 1001      | 2020-01-02 |
      | 1001      | 2020-01-03 |
    And I have a valid get cvp audio recordings by cloud room request for <CloudRoom> <Date>
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And <Results> audio recordings from cvp are retrieved
    Examples:
      | CloudRoom | Date       | Results |
      | 1001      | 2020-01-01 | 2       |
      | 1001      | 2020-01-02 | 3       |
      | 1001      | 2020-01-03 | 1       |
      | 1001      | 2020-01-04 | 0       |

  @VIH-6385
  Scenario Outline: Get Audio Recordings for CVP by date with case reference number
    Given Cvp has audio recordings
      | Date       | CaseReference |
      | 2020-01-01 | MyReference1  |
      | 2020-01-01 | MyReference1  |
      | 2020-01-02 | MyReference3  |
      | 2020-01-02 | MyReference4  |
      | 2020-01-02 | MyReference5  |
    And I have a valid get cvp audio recordings by date request for <Date> <CaseReference>
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And <Results> audio recordings from cvp are retrieved
    Examples:
      | Date       | CaseReference         | Results |
      | 2020-01-01 | MyReference1          | 2       |
      | 2020-01-02 | MyReference3          | 1       |
      | 2020-01-02 | MyReference4          | 1       |
      | 2020-01-02 | MyReference5          | 1       |
      | 2020-01-02 | MyReference99999xxxxx | 0       |
      | 2020-01-09 | NoExist               | 0       |
