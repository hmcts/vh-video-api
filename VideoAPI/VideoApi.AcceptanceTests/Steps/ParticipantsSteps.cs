using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using Testing.Common.Helper.Builders.Api;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Common.Helpers;
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
        private readonly CommonSteps _commonSteps;
        private const string ParticipantUsernameKey = "ParticipantUsername";

        public ParticipantsSteps(TestContext injectedContext, ScenarioContext scenarioContext, CommonSteps commonSteps)
        {
            _context = injectedContext;
            _scenarioContext = scenarioContext;
            _commonSteps = commonSteps;
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

        [Given(@"I have an update participant details request")]
        public void GivenIHaveAnUpdateParticipantRequest()
        {
            var participant = _context.NewConference.Participants.First(x => x.UserRole == UserRole.Individual);
            var request = new UpdateParticipantRequest()
            {
                Fullname = $"Updated {participant.Name}",
                DisplayName = $"Updated {participant.DisplayName}",
                Representee = $"Updated {participant.Representee}"
            };
            _scenarioContext.Add(ParticipantUsernameKey, participant.Username);
            _context.Request = _context.Patch(_endpoints.UpdateParticipantFromConference(_context.NewConferenceId, participant.Id), request);
        }

        [Given(@"the participant has a self test score")]
        public void GivenTheParticipantHasASelfTestScore()
        {
            GivenIHaveAnUpdateSelfTestScoreResultRequest();
            _commonSteps.WhenISendTheRequestToTheEndpoint();
            _commonSteps.ThenTheResponseShouldHaveTheStatusAndSuccessStatus(HttpStatusCode.NoContent, true);
        }

        [Given(@"I have an update self test score result request")]
        public void GivenIHaveAnUpdateSelfTestScoreResultRequest()
        {
            var participant = _context.NewConference.Participants.First(x => x.UserRole == UserRole.Individual);
            var request = new TestScoreResultRequest() { Score = TestScore.Good, Passed = true };
            _scenarioContext.Add(ParticipantUsernameKey, participant.Username);
            _context.Request = _context.Post(_endpoints.UpdateParticipantSelfTestScore(_context.NewConferenceId, participant.Id), request);
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
            var exists = conference.Participants.Any(participant => participant.Username.ToLower().Equals(_scenarioContext.Get<string>(ParticipantUsernameKey).ToLower()));
            if (state.Equals("added"))
            {
                exists.Should().BeTrue();
            } 
            else if (state.Equals("removed"))
            {
                exists.Should().BeFalse();
            } 
            else if (state.Equals("updated"))
            {
                var participant = conference.Participants.First(x =>
                    x.Username.Equals(_scenarioContext.Get<string>(ParticipantUsernameKey)));

                participant.Name.Should().Contain("Updated");
                participant.DisplayName.Should().Contain("Updated");
                participant.Representee.Should().Contain("Updated");
            }
        }

        [Then(@"the score should be good")]
        public void ThenTheScoreShouldBeGood()
        {
            var result = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<TestCallScoreResponse>(_context.Response.Content);
            result.Score.Should().Be(TestScore.Good);
            result.Passed.Should().BeTrue();
        }
    }
}
