Feature: Healthcheck
  In order to keep VH data up to date
  As an API service
  I want to check the Api health

  Scenario: check the Api Health to ensure that end points are up and running
    Given I make a call to the healthcheck endpoint
    When I send the request to the endpoint
    Then the response should have the status ok and success status True

  