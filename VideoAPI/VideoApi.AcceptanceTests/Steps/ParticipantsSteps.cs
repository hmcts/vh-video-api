using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using Testing.Common.Helper.Builders.Api;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Domain.Enums;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class ParticipantsSteps : BaseSteps
    {
        private readonly TestContext _context;
        private readonly ScenarioContext _scenarioContext;
        private readonly ParticipantsEndpoints _endpoints = new ApiUriFactory().ParticipantsEndpoints;
        private const string ParticipantUsernameKey = "ParticipantUsername";

        public ParticipantsSteps(TestContext injectedContext, ScenarioContext scenarioContext)
        {
            _context = injectedContext;
            _scenarioContext = scenarioContext;
        }

        [Given(@"I have an add participant to a valid conference request")]
        public void GivenIHaveAnAddParticipantToAValidConferenceRequest()
        {
            var request = new AddParticipantsToConferenceRequest
                { Participants = new List<ParticipantRequest> { new ParticipantRequestBuilder(UserRole.Individual).Build() } };
            _scenarioContext.Add(ParticipantUsernameKey, request.Participants.First().Username);
            _context.Request = _context.Put(_endpoints.AddParticipantsToConference(_context.NewConferenceId), request);
        }

        [Given(@"I have an remove participant from a valid conference request")]
        public void GivenIHaveAnRemoveParticipantFromAValidConferenceRequest()
        {
            _scenarioContext.Add(ParticipantUsernameKey, _context.NewConference.Participants.Last().DisplayName);
            _context.Request = _context.Delete(_endpoints.RemoveParticipantFromConference(_context.NewConferenceId, _context.NewConference.Participants.Last().Id));
        }

        [Then(@"the participant is (.*)")]
        public void ThenTheParticipantIsAdded(string state)
        {
            var endpoints = new ApiUriFactory().ConferenceEndpoints;
            _context.Request = _context.Get(endpoints.GetConferenceDetailsById(_context.NewConferenceId));
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.IsSuccessful.Should().BeTrue();
            var conference = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<ConferenceDetailsResponse>(_context.Response.Content);
            conference.Should().NotBeNull();
            var exists = conference.Participants.Any(participant => participant.Username == _scenarioContext.Get<string>(ParticipantUsernameKey));
            if (state.Equals("added"))
            {
                exists.Should().BeTrue();
            }
            if (state.Equals("removed"))
            {
                exists.Should().BeFalse();
            }
        }
    }
}
