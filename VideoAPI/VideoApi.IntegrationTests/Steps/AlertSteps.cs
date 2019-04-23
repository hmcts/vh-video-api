using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Contexts;
using VideoApi.IntegrationTests.Helper;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public class AlertSteps : StepsBase
    {
        private readonly AlertEndpoints _endpoints = new ApiUriFactory().AlertEndpoints;
        
        public AlertSteps(ApiTestContext apiTestContext) : base(apiTestContext)
        {
        }

        [Given(@"I have a (.*) add alert to conference request")]
        [Given(@"I have an (.*) add alert to conference request")]
        public async Task GivenIHaveAnAddAlertRequest(Scenario scenario)
        {
            var seededConference = await ApiTestContext.TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            ApiTestContext.NewConferenceId = seededConference.Id;
            
            var conferenceId = seededConference.Id;
            var request = new AddAlertRequest();
            switch (scenario)
            {
                case Scenario.Valid:
                    request.Body = "Automated API Test";
                    request.Type = AlertType.Hearing;
                    break;
                case Scenario.Invalid:
                    request.Body = string.Empty;
                    request.Type = null;
                    break;
                case Scenario.Nonexistent:
                    request.Body = "Automated API Test";
                    request.Type = AlertType.Hearing;
                    conferenceId = Guid.NewGuid();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            
            ApiTestContext.Uri = _endpoints.AddAlertToConference(conferenceId);
            ApiTestContext.HttpMethod = HttpMethod.Put;
            var jsonBody = ApiRequestHelper.SerialiseRequestToSnakeCaseJson(request);
            ApiTestContext.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have a (.*) get pending alerts request")]
        [Given(@"I have an (.*) get pending alerts request")]
        public async Task GivenIHaveAGetPendingAlertsRequest(Scenario scenario)
        {
            Guid conferenceId;
            switch (scenario)
            {
                case Scenario.Valid:
                    var seededConference = await SeedConferenceWithAlerts();
                    TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
                    ApiTestContext.NewConferenceId = seededConference.Id;
                    conferenceId = seededConference.Id;
                    break;
                case Scenario.Nonexistent:
                    conferenceId = Guid.NewGuid();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null);
            }
            
            ApiTestContext.Uri = _endpoints.GetPendingAlerts(conferenceId);
            ApiTestContext.HttpMethod = HttpMethod.Get;
        }

        [Then(@"the list of alerts should be retrieved")]
        public async Task ThenTheListOfAlertsShouldBeRetrieved()
        {
            var json = await ApiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var alerts = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<AlertResponse>>(json);
            alerts.Should().NotBeNullOrEmpty();
            foreach (var alert in alerts)
            {
                alert.Id.Should().BeGreaterThan(0);
                alert.Body.Should().NotBeNullOrWhiteSpace();
                alert.Type.Should().BeOfType<AlertType>();
            }
        }
        
        private async Task<Conference> SeedConferenceWithAlerts()
        {
            const string body = "Automated Test Complete Alert";
            const string updatedBy = "test@automated.com";
            
            var judgeAlertDone = new Alert(body, AlertType.Judge);
            judgeAlertDone.CompleteTask(updatedBy);
            var participantAlertDone = new Alert(body, AlertType.Participant);
            participantAlertDone.CompleteTask(updatedBy);
            var hearingAlertDone = new Alert(body, AlertType.Hearing);
            hearingAlertDone.CompleteTask(updatedBy);
            
            var conference = new ConferenceBuilder(true)
                .WithParticipant(UserRole.Individual, "Claimant")
                .Build();
            
            conference.AddAlert(AlertType.Judge, body);
            conference.AddAlert(AlertType.Participant, body);
            conference.AddAlert(AlertType.Hearing, body);
            conference.AddAlert(AlertType.Participant, body);

            conference.GetAlerts()[0].CompleteTask(updatedBy);
            
            return await ApiTestContext.TestDataManager.SeedConference(conference);
        }
        
    }
}