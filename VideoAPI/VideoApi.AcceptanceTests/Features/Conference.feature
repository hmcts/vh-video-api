Feature: Conference
	In order to manage conferences
	As an api service
	I want to be able to create, retrieve, update and delete conferences

Scenario: Get conference details by username
    Given I have a conference
	And I have a get details for a conference request by username with a valid username
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the summary of conference details should be retrieved

Scenario: Create conference
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

Scenario: Delete conference
    Given I have a conference
    And I have a valid delete conference request
    When I send the request to the endpoint
    Then the response should have the status NoContent and success status True
    And the conference should be removed

Scenario: Get conference details by hearing id
    Given I have a conference
	And I have a get details for a conference request by hearing id with a valid username
    When I send the request to the endpoint
    Then the response should have the status OK and success status True
    And the conference details should be retrieved
