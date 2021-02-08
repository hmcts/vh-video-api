@health @VIH-3806 @VIH-5894
Feature: Health Check
  In order to assess the status of the service
  As an api service
  I want to be able to request the health of the video api

  Scenario: Get the health of the api
    Given I have a get health request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the application version should be retrieved
    And the Wowza health should be retrieved
