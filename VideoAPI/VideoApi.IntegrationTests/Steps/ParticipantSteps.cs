using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using Testing.Common.Helper.Builders.Api;
using VideoApi.Contract.Requests;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Contexts;
using VideoApi.IntegrationTests.Helper;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public sealed class ParticipantSteps : StepsBase
    {
        private readonly ParticipantsEndpoints _endpoints = new ApiUriFactory().ParticipantsEndpoints;

        public ParticipantSteps(ApiTestContext apiTestContext) : base(apiTestContext)
        {
        }

        [Given(@"I have an update participant status request for a (.*) conference")]
        [Given(@"I have an update participant status request for an (.*) conference")]
        public async Task GivenIHaveAnUpdateParticipantStatusRequest(Scenario scenario)
        {
            Guid conferenceId;
            long participantId;
            UpdateParticipantStatusRequest request;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    var seededConference = await ApiTestContext.TestDataManager.SeedConference();
                    TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
                    ApiTestContext.NewConferenceId = seededConference.Id;
                    conferenceId = seededConference.Id;
                    participantId = seededConference.GetParticipants().First().Id;
                    request = new UpdateParticipantStatusRequest {State = ParticipantState.InHearing};
                    break;
                }
                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    participantId = 1;
                    request = new UpdateParticipantStatusRequest {State = ParticipantState.InHearing};
                    break;
                case Scenario.Invalid:
                    conferenceId = Guid.Empty;
                    participantId = 1;
                    request = new UpdateParticipantStatusRequest();
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            ApiTestContext.Uri = _endpoints.UpdateParticipantStatus(conferenceId, participantId);
            ApiTestContext.HttpMethod = HttpMethod.Patch;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have an update participant status request for a (.*) participant")]
        [Given(@"I have an update participant status request for an (.*) participant")]
        public async Task GivenIHaveAnUpdateParticipantStatusRequestForParticipant(Scenario scenario)
        {
            var seededConference = await ApiTestContext.TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            var conferenceId = seededConference.Id;
            long participantId;
            UpdateParticipantStatusRequest request;
            switch (scenario)
            {
                case Scenario.Nonexistent:
                    participantId = long.MaxValue;
                    request = new UpdateParticipantStatusRequest {State = ParticipantState.InHearing};
                    break;
                case Scenario.Negative:
                    participantId = seededConference.GetParticipants().First().Id;
                    request = new UpdateParticipantStatusRequest();
                    break;
                case Scenario.Invalid:
                    participantId = long.MinValue;
                    request = new UpdateParticipantStatusRequest {State = ParticipantState.InHearing};
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            ApiTestContext.Uri = _endpoints.UpdateParticipantStatus(conferenceId, participantId);
            ApiTestContext.HttpMethod = HttpMethod.Patch;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have an add participant to a (.*) conference request")]
        [Given(@"I have an add participant to an (.*) conference request")]
        public async Task GivenIHaveAnAddParticipantToConferenceRequest(Scenario scenario)
        {
            Guid conferenceId;
            AddParticipantsToConferenceRequest request;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    var seededConference = await ApiTestContext.TestDataManager.SeedConference();
                    TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
                    ApiTestContext.NewConferenceId = seededConference.Id;
                    conferenceId = seededConference.Id;
                    request = new AddParticipantsToConferenceRequest
                        {Participants = new List<ParticipantRequest> {new ParticipantRequestBuilder().Build()}};
                    break;
                }
                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    request = new AddParticipantsToConferenceRequest
                        {Participants = new List<ParticipantRequest> {new ParticipantRequestBuilder().Build()}};
                    break;
                case Scenario.Invalid:
                    conferenceId = Guid.Empty;
                    request = new AddParticipantsToConferenceRequest
                        {Participants = new List<ParticipantRequest> {new ParticipantRequestBuilder().Build()}};
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            ApiTestContext.Uri = _endpoints.AddParticipantsToConference(conferenceId);
            ApiTestContext.HttpMethod = HttpMethod.Put;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have an add participant to a conference request with a (.*) body")]
        [Given(@"I have an add participant to a conference request with an (.*) body")]
        public async Task GivenIHaveAnAddParticipantToConferenceRequestWith(Scenario scenario)
        {
            var seededConference = await ApiTestContext.TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            ApiTestContext.NewConferenceId = seededConference.Id;
            var conferenceId = seededConference.Id;
            AddParticipantsToConferenceRequest request;
            switch (scenario)
            {
                case Scenario.Invalid:
                    request = new AddParticipantsToConferenceRequest {Participants = new List<ParticipantRequest>()};
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            ApiTestContext.Uri = _endpoints.AddParticipantsToConference(conferenceId);
            ApiTestContext.HttpMethod = HttpMethod.Put;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have an remove participant from a (.*) conference request")]
        [Given(@"I have an remove participant from an (.*) conference request")]
        public async Task GivenIHaveAnRemoveParticipantFromConferenceRequest(Scenario scenario)
        {
            Guid conferenceId;
            long participantId;
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    var seededConference = await ApiTestContext.TestDataManager.SeedConference();
                    TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
                    ApiTestContext.NewConferenceId = seededConference.Id;
                    conferenceId = seededConference.Id;
                    participantId = seededConference.GetParticipants().First().Id;
                    break;
                }
                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    participantId = 1;
                    break;
                case Scenario.Invalid:
                    conferenceId = Guid.Empty;
                    participantId = 1;
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            ApiTestContext.Uri = _endpoints.RemoveParticipantFromConference(conferenceId, participantId);
            ApiTestContext.HttpMethod = HttpMethod.Delete;
        }

        [Given(@"I have a remove participant from a conference request for a (.*) participant")]
        [Given(@"I have a remove participant from a conference request for an (.*) participant")]
        public async Task GivenIHaveAnRemoveParticipantFromConferenceRequestWithParticipant(Scenario scenario)
        {
            var seededConference = await ApiTestContext.TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            var conferenceId = seededConference.Id;
            long participantId;
            switch (scenario)
            {
                case Scenario.Nonexistent:
                    participantId = long.MaxValue;
                    break;
                case Scenario.Invalid:
                    participantId = long.MinValue;
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            ApiTestContext.Uri = _endpoints.RemoveParticipantFromConference(conferenceId, participantId);
            ApiTestContext.HttpMethod = HttpMethod.Delete;
        }
    }
}