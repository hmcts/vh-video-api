Feature: Conference
  In order to manage conferences
  As an api service
  I want to be able to create, retrieve, update and delete conferences

  Scenario: Update conference
	  Given I have a conference
	  And I have an update conference request
	  When I send the request to the endpoint
	  Then the response should have the status OK and success status True
	  And the conference details have been updated

  Scenario: Create conference
	  Given I have a valid book a new conference request
	  When I send the request to the endpoint
	  Then the response should have the status Created and success status True
	  And the conference details should be retrieved

  Scenario: Create a new conference with jvs endpoints
    Given I have a valid book a new conference request with jvs endpoints
    When I send the request to the endpoint
    Then the response should have the status Created and success status True
    And the conference details should be retrieved with jvs endpoints

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

  Scenario: Get conferences today for vho
	  Given I have a conference
	  And I have another conference
	  And I have a conference for tomorrow
	  And I have a get conferences today for a vho
	  When I send the request to the endpoint
	  Then the response should have the status OK and success status True
	  And a list containing only todays hearings conference details should be retrieved

  Scenario: Get conferences today for judge
	  Given I have a conference
	  And I have another conference
	  And I have a conference for tomorrow
	  And I have a get conferences today for a judge
	  When I send the request to the endpoint
	  Then the response should have the status OK and success status True
	  And a list containing only judge todays hearings conference details should be retrieved

  Scenario: Get conferences today for individual
	  Given I have a conference
	  And I have another conference
	  And I have a conference for tomorrow
	  And I have a get conferences today for an individual
	  When I send the request to the endpoint
	  Then the response should have the status OK and success status True
	  And a list containing only individual todays hearings conference details should be retrieved

  Scenario: Get conference details by hearing id
	  Given I have a conference
	  And I have a get details for a conference request by hearing id with a valid username
	  When I send the request to the endpoint
	  Then the response should have the status OK and success status True
	  And the conference details should be retrieved

  Scenario: Get expired conferences
	  Given I have a conference
	  And I have another conference
	  And I close the last created conference
	  And I have a get expired conferences request
	  When I send the request to the endpoint
	  Then the response should have the status OK and success status True
	  And I have an empty list of expired conferences

  Scenario: Close all conferences
	  Given I have a conference
	  And I have another conference
	  And I have a conference for tomorrow
	  And I close all conferences
	  And I have a get expired conferences request
	  When I send the request to the endpoint
	  Then the response should have the status OK and success status True
	  And a list not containing the closed hearings should be retrieved

  Scenario: Get audiorecording expired closed conferences
	  Given I have a conference with audiorecording
	  And I have another conference with no audiorecording
	  And I have a conference for tomorrow with audiorecording
	  And All conferences have started
	  And I have a get expired audiorecording conferences request
	  When I send the request to the endpoint
	  Then the response should have the status OK and success status True
	  And retrieved list should not include not expired hearings or without audiorecording

  @VIH-5827
  Scenario: Get the judges in hearings today
	  Given I have a conference
	  And the judge status is in hearing
	  And I have a get judges in hearings today
	  When I send the request to the endpoint
	  Then the response should have the status OK and success status True
	  And the Judges in hearings should be retrieved
