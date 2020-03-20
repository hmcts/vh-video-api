@VIH-3806 @health
Feature: Healthcheck
  In order to assess the status of the service
  As an api service
  I want to be able to request the health of the video api

  Scenario: Get the health of the user api
    Given I have a get health request
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the application version should be retrieved
