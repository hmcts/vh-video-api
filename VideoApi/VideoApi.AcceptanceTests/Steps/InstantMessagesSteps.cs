using System.Collections.Generic;
using System.Linq;
using System.Net;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using TechTalk.SpecFlow;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Contract.Enums;
using static Testing.Common.Helper.ApiUriFactory.InstantMessageEndpoints;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public class InstantMessagesSteps 
    {
        private readonly string _fromUsername;
        private const string MessageBody = "A message";
        private readonly TestContext _context;
        private readonly string _toUsername = "Receiver Username";
        private readonly string _nonExistentUser = "nonExistentUserName";

        public InstantMessagesSteps(TestContext injectedContext)
        {
            _context = injectedContext;
            _fromUsername = _context.Test.ConferenceResponse.Participants.First(x => x.UserRole == UserRole.Judge).DisplayName;
        }

        [Given(@"the conference has existing messages")]
        public void GivenTheConferenceHasExistingMessages()
        {
            CreateMessage();
        }

        [Given(@"I have a get chat messages request")]
        public void GivenIHaveAGetChatMessagesRequest()
        {
            _context.Request = _context.Get(GetInstantMessageHistory(_context.Test.ConferenceResponse.Id));
        }

        [Given(@"I have a get chat messages request for participant")]
        public void GivenIHaveAGetChatMessagesRequestForParticipant()
        {
            _context.Request = _context.Get(GetInstantMessageHistoryFor(_context.Test.ConferenceResponse.Id, _toUsername));
        }

        [Given(@"I have a get chat messages request for non existent participant")]
        public void GivenIHaveAGetChatMessagesRequestForNonExistentParticipant()
        {
            _context.Request = _context.Get(GetInstantMessageHistoryFor(_context.Test.ConferenceResponse.Id, _nonExistentUser));
        }

        [Given(@"I have a create chat messages request")]
        public void GivenIHaveACreateChatMessagesRequest()
        {
            var request = new AddInstantMessageRequest()
            {
                From = _fromUsername,
                MessageText = MessageBody,
                To = _toUsername
            };
            _context.Request = _context.Post(SaveInstantMessage(_context.Test.ConferenceResponse.Id), request);
        }

        [Given(@"I have a remove messages from a conference request")]
        public void GivenIHaveARemoveMessagesFromAConferenceRequest()
        {
            _context.Request = _context.Delete(RemoveInstantMessagesForConference(_context.Test.ConferenceResponse.Id));
        }


        [Then(@"the chat messages are retrieved")]
        public void ThenTheChatMessagesRetrieved()
        {
            var message = GetMessages().First();
            message.From.Should().Be(_fromUsername);
            message.MessageText.Should().Be(MessageBody);
            message.To.Should().Be(_toUsername);
        }

        [Then(@"the chat messages are retrieved for the participant")]
        public void ThenTheChatMessagesAreRetrievedForTheParticipant()
        {
            var message = GetMessagesForParticipant().First();
            message.From.Should().Be(_fromUsername);
            message.MessageText.Should().Be(MessageBody);
            message.To.Should().Be(_toUsername);
        }

        [Then(@"no chat messages are retrieved for the participant")]
        public void ThenNoChatMessagesAreRetrievedForTheParticipant()
        {
            var message = GetMessagesForNoParticipant();
            message.Count().Should().Be(0);
        }

        [Then(@"the chat messages are deleted")]
        public void ThenTheChatMessagesAreDeleted()
        {
            var message = GetMessages();
            message.Count().Should().Be(0);
        }

        private IEnumerable<InstantMessageResponse> GetMessages()
       {
            GivenIHaveAGetChatMessagesRequest();
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            return RequestHelper.Deserialise<List<InstantMessageResponse>>(_context.Response.Content);
        }

        private void CreateMessage()
        {
            GivenIHaveACreateChatMessagesRequest();
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        private IEnumerable<InstantMessageResponse> GetMessagesForParticipant()
        {
            GivenIHaveAGetChatMessagesRequestForParticipant();
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            return RequestHelper.Deserialise<List<InstantMessageResponse>>(_context.Response.Content);
        }

        private IEnumerable<InstantMessageResponse> GetMessagesForNoParticipant()
        {
            GivenIHaveAGetChatMessagesRequestForNonExistentParticipant();
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.StatusCode.Should().Be(HttpStatusCode.OK);
            return RequestHelper.Deserialise<List<InstantMessageResponse>>(_context.Response.Content);
        }
    }
}
