Feature: Audio Recording
  In order to enable the audio recordings of hearings
  As an api service
  I want to enable CRUD processes for audio recordings


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
