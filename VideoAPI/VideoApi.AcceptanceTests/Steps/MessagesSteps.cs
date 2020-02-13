using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Common.Helpers;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public class MessagesSteps : BaseSteps
    {
        private const string PersonAEmail = "PersonA@email.com";
        private const string PersonBEmail = "PersonB@email.com";
        private const string Message = "A message";
        private readonly TestContext _context;
        private readonly MessageEndpoints _endpoints = new ApiUriFactory().MessageEndpoints;

        public MessagesSteps(TestContext injectedContext)
        {
            _context = injectedContext;
        }

        [Given(@"the conference has existing messages")]
        public void GivenTheConferenceHasExistingMessages()
        {
            CreateMessage();
        }

        [Given(@"I have a get chat messages request")]
        public void GivenIHaveAGetChatMessagesRequest()
        {
            _context.Request = _context.Get(_endpoints.GetMessages(_context.NewConferenceId));
        }

        [Given(@"I have a create chat messages request")]
        public void GivenIHaveACreateChatMessagesRequest()
        {
            var request = new AddMessageRequest()
            {
                From = PersonAEmail,
                To = PersonBEmail,
                MessageText = Message
            };
            _context.Request = _context.Post(_endpoints.SaveMessage(_context.NewConferenceId), request);
        }

        [Then(@"the chat messages are retrieved")]
        public void ThenTheChatMessagesRetrieved()
        {
            var messages = GetMessages();
            messages.First().From.Should().Be(PersonAEmail);
            messages.First().To.Should().Be(PersonBEmail);
            messages.First().MessageText.Should().Be(Message);
        }

        private IEnumerable<MessageResponse> GetMessages()
        {
            GivenIHaveAGetChatMessagesRequest();
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            return ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<MessageResponse>>(_context.Response.Content);
        }

        private void CreateMessage()
        {
            GivenIHaveACreateChatMessagesRequest();
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.StatusCode.Should().Be(HttpStatusCode.Created);
        }
    }
}
