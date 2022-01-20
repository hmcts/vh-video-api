Feature: Instant Messages
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

@VIH-5490
Scenario: Remove messages for an existing conference
Given I have a conference
And the conference has existing messages
And I have a remove messages from a conference request
When I send the request to the endpoint
Then the response should have the status OK and success status True
And the chat messages are deleted

@VIH-6021
Scenario: Get chat messages for a participant
  Given I have a conference
  And the conference has existing messages
  And I have a get chat messages request for participant
  When I send the request to the endpoint
  Then the response should have the status OK and success status True
  And the chat messages are retrieved for the participant

Scenario: Get chat messages for a non existent participant
  Given I have a conference
  And the conference has existing messages
  And I have a get chat messages request for non existent participant
  When I send the request to the endpoint
  Then the response should have the status OK and success status True
  And no chat messages are retrieved for the participant
