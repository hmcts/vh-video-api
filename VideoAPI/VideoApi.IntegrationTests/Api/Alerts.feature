Feature: Alerts
  In order to track alerts to be actions
  As an API service
  I want to raise new alerts and retrieve pending alerts

  Scenario: Successfully add an alert to a conference
    Given I have a valid add alert to conference request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True

  Scenario: Unable to add an alert to a conference that does not exist
    Given I have a nonexistent add alert to conference request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False

  Scenario: Unable to add an alert to a conference with invalid details
    Given I have an invalid add alert to conference request
    When I send the request to the endpoint
    Then the response should have the status BadRequest and success status False

  Scenario: Get pending alerts for a conference
    Given I have a valid get pending alerts request
    When I send the request to the endpoint
    Then the response should have the status Ok and success status True
    And the list of alerts should be retrieved

  Scenario: Unable to get alerts for a conference that does not exist
    Given I have a nonexistent get pending alerts request
    When I send the request to the endpoint
    Then the response should have the status NotFound and success status False