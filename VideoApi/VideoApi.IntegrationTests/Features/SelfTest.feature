Feature: Self Test
	In order to avoid setting old values for the Supplier self test
	As an api service
	I want to be able to retrieve the pexip service configuration settings

Scenario: Get pexip service configuration
	Given I have a self test request
  When I send the request to the endpoint
  Then the response should have the status OK and success status True
	And the pexip service configuration should be retrieved
