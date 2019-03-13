Feature: Conference
	In order to manage conferences
	As an api service
	I want to be able to create, update and retrieve conference data

Scenario: Create a new conference
    Given I have a valid book a new conference request
    When I send the request to the endpoint
    Then the response should have the status Created and success status True
    And the conference details should be retrieved

Scenario: Get conference details
    Given I have a conference
	And I have a get details for a conference request with a valid conference id
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the conference details should be retrieved

Scenario: Update conference details
    Given I have a conference
    And I have a valid update conference status request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True	
	And the conference details have been updated