Feature: Healthcheck
  In order to keep VH data up to date
  As an API service
  I want to check the Api health

  Scenario: Get the health of the video api
    Given I have a get health request
    When I send the request to the endpoint
    Then the response should have the status ok and success status True
    And the application version should be retrieved
    And the Wowza health should be retrieved
  
