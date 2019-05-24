using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Assertions;
using Testing.Common.Helper;
using Testing.Common.Helper.Builders.Api;
using VideoApi.AcceptanceTests.Contexts;
using VideoApi.Common.Helpers;
using VideoApi.Contract.Responses;

namespace VideoApi.AcceptanceTests.Steps
{
    [Binding]
    public sealed class ConferenceSteps : BaseSteps
    {
        private readonly TestContext _context;
        private readonly ScenarioContext _scenarioContext;
        private readonly ConferenceEndpoints _endpoints = new ApiUriFactory().ConferenceEndpoints;
        private const string OriginalStatusKey = "originalConferenceStatus";

        public ConferenceSteps(TestContext injectedContext, ScenarioContext scenarioContext)
        {
            _context = injectedContext;
            _scenarioContext = scenarioContext;
        }

        [Given(@"I have a get details for a conference request by username with a valid username")]
        public void GivenIHaveAGetDetailsForAConferenceRequestByUsernameWithAValidUsername()
        {
            _context.Request = _context.Get(_endpoints.GetConferenceDetailsByUsername(_context.NewConference.Participants.First().Username));
        }

        [Given(@"I have a valid book a new conference request")]
        public void GivenIHaveAValidBookANewConferenceRequest()
        {
            CreateNewConferenceRequest();
        }

        private void CreateNewConferenceRequest()
        {
            _context.NewHearingRefId = Guid.NewGuid();
            var request = new BookNewConferenceRequestBuilder()
                .WithJudge()
                .WithRepresentative("Claimant").WithIndividual("Claimant")
                .WithRepresentative("Defendant").WithIndividual("Defendant")
                .WithVideoHearingsOfficer()
                .WithHearingRefId(_context.NewHearingRefId)
                .Build();
            _context.Request = _context.Post(_endpoints.BookNewConference, request);
        }

        [Given(@"I have a conference")]
        public void GivenIHaveAConference()
        {
            CreateNewConferenceRequest();
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.IsSuccessful.Should().BeTrue("New conference is created");
            var conference = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<ConferenceDetailsResponse>(_context.Response.Content);
            conference.Should().NotBeNull();
            _context.NewConferenceId = conference.Id;
            _context.NewConference = conference;
            _scenarioContext.Add(OriginalStatusKey, conference.CurrentStatus);
        }

        [Given(@"I have a get details for a conference request with a valid conference id")]
        public void GivenIHaveAGetDetailsForAConferenceRequestWithAValidConferenceId()
        {
            _context.Request = _context.Get(_endpoints.GetConferenceDetailsById(_context.NewConferenceId));
        }

        [Given(@"I have a valid delete conference request")]
        public void GivenIHaveAValidDeleteConferenceRequest()
        {
            _context.Request = _context.Delete(_endpoints.RemoveConference(_context.NewConferenceId));
        }

        [Given(@"I have a get details for a conference request by hearing id with a valid username")]
        public void GivenIHaveAGetDetailsForAConferenceRequestByHearingIdWithAValidUsername()
        {
            _context.Request = _context.Get(_endpoints.GetConferenceByHearingRefId(_context.NewHearingRefId));
        }

        [Then(@"the conference details have been updated")]
        public void ThenICanSeeTheConferenceDetailsHaveBeenUpdated()
        {
            _context.Request = _context.Get(_endpoints.GetConferenceDetailsById(_context.NewConferenceId));
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.IsSuccessful.Should().BeTrue("Conference details are retrieved");
            var conference = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<ConferenceDetailsResponse>(_context.Response.Content);
            conference.Should().NotBeNull();
            conference.CurrentStatus.Should().NotBe(_scenarioContext.Get<string>(OriginalStatusKey));
        }

        [Then(@"the conference details should be retrieved")]
        public void ThenTheConferenceDetailsShouldBeRetrieved()
        {
            var conference = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<ConferenceDetailsResponse>(_context.Json);
            conference.Should().NotBeNull();
            _context.NewConferenceId = conference.Id;
            AssertConferenceDetailsResponse.ForConference(conference);
        }

        [Then(@"the summary of conference details should be retrieved")]
        public void ThenTheSummaryOfConferenceDetailsShouldBeRetrieved()
        {
            var conferences = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<ConferenceSummaryResponse>>(_context.Json);
            conferences.Should().NotBeNull();
            _context.NewConferenceId = conferences.First().Id;
            foreach (var conference in conferences)
            {
                AssertConferenceSummaryResponse.ForConference(conference);
                foreach (var participant in conference.Participants)
                {
                    AssertParticipantSummaryResponse.ForParticipant(participant);
                }
            }
        }

        [Then(@"the conference should be removed")]
        public void ThenTheConferenceShouldBeRemoved()
        {
            _context.Request = _context.Get(_endpoints.GetConferenceDetailsById(_context.NewConferenceId));
            _context.Response = _context.Client().Execute(_context.Request);
            _context.Response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            _context.NewConferenceId = Guid.Empty;
        }        
    }
}
