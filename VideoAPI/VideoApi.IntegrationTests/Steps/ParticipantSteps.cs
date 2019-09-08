using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using Testing.Common.Helper.Builders.Api;
using VideoApi.Common.Helpers;
using VideoApi.Contract.Requests;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Contexts;
using VideoApi.IntegrationTests.Helper;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public sealed class ParticipantSteps : StepsBase
    {
        private readonly ConferenceTestContext _conferenceTestContext;
        private readonly ParticipantsEndpoints _endpoints = new ApiUriFactory().ParticipantsEndpoints;

        public ParticipantSteps(ApiTestContext apiTestContext, ConferenceTestContext conferenceTestContext) : base(apiTestContext)
        {
            _conferenceTestContext = conferenceTestContext;
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
                    {
                        Participants = new List<ParticipantRequest>
                            {new ParticipantRequestBuilder(UserRole.Individual).Build()}
                    };
                    break;
                }

                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    request = new AddParticipantsToConferenceRequest
                    {
                        Participants = new List<ParticipantRequest>
                            {new ParticipantRequestBuilder(UserRole.Individual).Build()}
                    };
                    break;
                case Scenario.Invalid:
                    conferenceId = Guid.Empty;
                    request = new AddParticipantsToConferenceRequest
                    {
                        Participants = new List<ParticipantRequest>
                            {new ParticipantRequestBuilder(UserRole.Individual).Build()}
                    };
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            ApiTestContext.Uri = _endpoints.AddParticipantsToConference(conferenceId);
            ApiTestContext.HttpMethod = HttpMethod.Put;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have an update participant to a (.*) conference request")]
        public async Task GivenIHaveAnUpdateParticipantToConferenceRequest(Scenario scenario)
        {
            Guid conferenceId;
            UpdateParticipantRequest request;
            Guid participantId = Guid.Empty; 
            switch (scenario)
            {
                case Scenario.Valid:
                {
                    var seededConference = await ApiTestContext.TestDataManager.SeedConference();
                    TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
                    ApiTestContext.NewConferenceId = seededConference.Id;
                    conferenceId = seededConference.Id; 
                    participantId = seededConference.Participants.First().Id;
                    request = new UpdateParticipantRequest
                    {
                        Fullname = "Automation_Mr Test_Fullname",
                        DisplayName = "Automation_Test_Displayname",
                        Representee = "Automation_Test_Representee"
                    };
                    break;
                }

                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    participantId = Guid.NewGuid();
                    request = new UpdateParticipantRequest
                    {
                        Fullname = "Automation_Mr Test_Fullname",
                        DisplayName = "Automation_Test_Displayname",
                        Representee = "Automation_Test_Representee"
                    };
                    break;
                case Scenario.Invalid:
                    conferenceId = Guid.Empty;
                    request = new UpdateParticipantRequest
                    {
                        Fullname = "Automation_Mr Test_Fullname",
                        DisplayName = "Automation_Test_Displayname",
                        Representee = "Automation_Test_Representee"
                    };
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            ApiTestContext.Uri = _endpoints.UpdateParticipantFromConference(conferenceId, participantId);
            ApiTestContext.HttpMethod = HttpMethod.Patch;
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
            Guid participantId;
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
                    participantId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    conferenceId = Guid.Empty;
                    participantId = Guid.Empty;
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
            ApiTestContext.NewConferenceId = seededConference.Id;
            Guid participantId;
            switch (scenario)
            {
                case Scenario.Nonexistent:
                    participantId = Guid.NewGuid();
                    break;
                case Scenario.Invalid:
                    participantId = Guid.Empty;
                    break;

                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }

            ApiTestContext.Uri = _endpoints.RemoveParticipantFromConference(conferenceId, participantId);
            ApiTestContext.HttpMethod = HttpMethod.Delete;
        }

        [Given("I have a (.*) get self test score request")]
        public void GivenIHaveAGetSelfTestScoreRequest(Scenario scenario)
        {
            Guid conferenceId = _conferenceTestContext.SeededConference.Id;
            Guid participantId;
            switch (scenario)
            {
                case Scenario.Nonexistent:
                    participantId = conferenceId;
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            
            ApiTestContext.Uri = _endpoints.GetTestCallResultForParticipant(conferenceId, participantId);
            ApiTestContext.HttpMethod = HttpMethod.Get;
        }
    }
}