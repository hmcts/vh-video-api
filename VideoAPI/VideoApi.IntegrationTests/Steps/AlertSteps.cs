using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using VideoApi.Contract.Requests;
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
    }
}