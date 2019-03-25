using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Faker;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Testing.Common.Assertions;
using Testing.Common.Helper;
using Testing.Common.Helper.Builders.Api;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.DAL;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Contexts;
using VideoApi.IntegrationTests.Helper;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public sealed class ConferenceSteps : StepsBase
    {
        private readonly ConferenceEndpoints _endpoints = new ApiUriFactory().ConferenceEndpoints;

        public ConferenceSteps(ApiTestContext apiTestContext) : base(apiTestContext)
        {
        }

        [Given(@"I have a get details for a conference request by username with a (.*) username")]
        [Given(@"I have a get details for a conference request by username with an (.*) username")]
        public async Task GivenIHaveAGetDetailsForAConferenceRequestByUsernameWithAValidUsername(Scenario scenario)
        {
            string username;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    var seededConference = await ApiTestContext.TestDataManager.SeedConference();
                    TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
                    ApiTestContext.NewConferenceId = seededConference.Id;
                    username = seededConference.Participants.First().Username;
                    break;
                }
                case Scenario.Nonexistent:
                    username = Internet.Email();
                    break;
                case Scenario.Invalid:
                    username = "invalidemail";
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            ApiTestContext.Uri = _endpoints.GetConferenceDetailsByUsername(username);
            ApiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a get details for a conference request with a (.*) conference id")]
        [Given(@"I have a get details for a conference request with an (.*) conference id")]
        public async Task GivenIHaveAGetConferenceDetailsRequest(Scenario scenario)
        {
            Guid conferenceId;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    var seededConference = await ApiTestContext.TestDataManager.SeedConference();
                    TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
                    ApiTestContext.NewConferenceId = seededConference.Id;
                    conferenceId = seededConference.Id;
                    break;
                }
                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    conferenceId = Guid.Empty;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            ApiTestContext.Uri = _endpoints.GetConferenceDetailsById(conferenceId);
            ApiTestContext.HttpMethod = HttpMethod.Get;
        }
        
        [Given(@"I have a get details for a conference request with a (.*) hearing ref id")]
        [Given(@"I have a get details for a conference request with an (.*) hearing ref id")]
        public async Task GivenIHaveAGetConferenceDetailsByHearingRefIdRequest(Scenario scenario)
        {
            Guid hearingRefId;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    var seededConference = await ApiTestContext.TestDataManager.SeedConference();
                    TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
                    ApiTestContext.NewConferenceId = seededConference.Id;
                    hearingRefId = seededConference.HearingRefId;
                    break;
                }
                case Scenario.Nonexistent:
                    hearingRefId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    hearingRefId = Guid.Empty;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            ApiTestContext.Uri = _endpoints.GetConferenceByHearingRefId(hearingRefId);
            ApiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Given(@"I have a (.*) book a new conference request")]
        [Given(@"I have an (.*) book a new conference request")]
        public void GivenIHaveABookANewConferenceRequest(Scenario scenario)
        {
            var request = new BookNewConferenceRequestBuilder().WithJudge()
                .WithRepresentative("Claimant").WithIndividual("Claimant")
                .WithRepresentative("Defendant").WithIndividual("Defendant")
                .WithVideoHearingsOfficer().Build();
            if (scenario == Scenario.Invalid)
            {
                request.Participants = new List<ParticipantRequest>();
                request.HearingRefId = Guid.Empty;
                request.CaseType = string.Empty;
                request.CaseNumber = string.Empty;
                request.ScheduledDuration = 0;
                request.ScheduledDateTime = DateTime.Today.AddDays(-5);
            }

            ApiTestContext.Uri = _endpoints.BookNewConference;
            ApiTestContext.HttpMethod = HttpMethod.Post;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have a (.*) update conference status request")]
        [Given(@"I have an (.*) update conference status request")]
        public async Task GivenIHaveAnUpdateConferenceStatusRequest(Scenario scenario)
        {
            Guid conferenceId;
            UpdateConferenceStatusRequest request;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    var seededConference = await ApiTestContext.TestDataManager.SeedConference();
                    TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
                    ApiTestContext.NewConferenceId = seededConference.Id;
                    conferenceId = seededConference.Id;
                    request = new UpdateConferenceStatusRequest {State = ConferenceState.Paused};
                    break;
                }
                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    request = new UpdateConferenceStatusRequest {State = ConferenceState.Paused};
                    break;
                case Scenario.Invalid:
                    conferenceId = Guid.Empty;
                    request = new UpdateConferenceStatusRequest();
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            ApiTestContext.Uri = _endpoints.UpdateConferenceStatus(conferenceId);
            ApiTestContext.HttpMethod = HttpMethod.Patch;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have a invalid update conference status request for an existing conference")]
        public async Task GivenIHaveAnInvalidUpdateConferenceStatusRequestForAnExistentConference()
        {
            var seededConference = await ApiTestContext.TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            ApiTestContext.NewConferenceId = seededConference.Id;
            var conferenceId = seededConference.Id;
            var request = new UpdateConferenceStatusRequest();
            ApiTestContext.Uri = _endpoints.UpdateConferenceStatus(conferenceId);
            ApiTestContext.HttpMethod = HttpMethod.Patch;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }      
        
        [Given(@"I have a (.*) remove conference request")]
        [Given(@"I have an (.*) remove conference request")]
        public async Task GivenIHaveAValidRemoveHearingRequest(Scenario scenario)
        {
            Guid conferenceId;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    var seededConference = await ApiTestContext.TestDataManager.SeedConference();
                    TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
                    ApiTestContext.NewConferenceId = seededConference.Id;
                    conferenceId = seededConference.Id;
                    break;
                }
                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    conferenceId = Guid.Empty;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            ApiTestContext.Uri = _endpoints.RemoveConference(conferenceId);
            ApiTestContext.HttpMethod = HttpMethod.Delete;
        }

        [Then(@"the conference details should be retrieved")]
        public async Task ThenAConferenceDetailsShouldBeRetrieved()
        {
            var json = await ApiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var conference = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<ConferenceDetailsResponse>(json);
            conference.Should().NotBeNull();
            ApiTestContext.NewConferenceId = conference.Id;
            AssertConferenceDetailsResponse.ForConference(conference);
        }

        [Then(@"the summary of conference details should be retrieved")]
        public async Task ThenTheSummaryOfConferenceDetailsShouldBeRetrieved()
        {
            var json = await ApiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var conferences = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<ConferenceSummaryResponse>>(json);
            conferences.Should().NotBeNull();
            ApiTestContext.NewConferenceId = conferences.First().Id;
            foreach (var conference in conferences)
            {
                AssertConferenceSummaryResponse.ForConference(conference);
            }
        }

        [Then(@"the conference should be removed")]
        public async Task ThenTheHearingShouldBeRemoved()
        {
            Conference removedConference;
            using (var db = new VideoApiDbContext(ApiTestContext.VideoBookingsDbContextOptions))
            {
                removedConference =
                    await db.Conferences.SingleOrDefaultAsync(x => x.Id == ApiTestContext.NewConferenceId);
            }
            removedConference.Should().BeNull();
            ApiTestContext.NewConferenceId = Guid.Empty;
        }

        [Then(@"an empty list is retrieved")]
        public async Task ThenAnEmptyListIsRetrieved()
        {
            var json = await ApiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var conferences = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<ConferenceSummaryResponse>>(json);
            conferences.Should().BeEmpty();
        }

    }
}