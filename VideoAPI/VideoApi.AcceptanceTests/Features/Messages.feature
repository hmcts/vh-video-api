Feature: Messages
  In order to manage IM messages
  As an api service
  I want to be able to create, retrieve, update and delete messages

@VIH-5489
Scenario: Get chat messages
  Given I have a conference
	And the conference has existing messages
  And I have a get chat messages request
  When I send the request to the endpoint
  Then the response should have the status OK and success status True
  And the chat messages are retrieved

@VIH-5495
Scenario: Create chat messages
  Given I have a conference
  And I have a create chat messages request
  When I send the request to the endpoint
  Then the response should have the status OK and success status True
