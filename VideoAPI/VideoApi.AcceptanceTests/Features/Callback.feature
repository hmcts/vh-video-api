Feature: Callback
  In order to keep VH data up to date
  As an API service
  I want to handle external events

Scenario: Add conference event
	Given I have a conference
    And I have a valid conference event request for event type Transfer
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
	And the status is updated
