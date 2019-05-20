using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow;
using VideoApi.IntegrationTests.Contexts;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public sealed class CommonSteps : StepsBase
    {
        private readonly ApiTestContext _apiTestContext;
        private readonly ConferenceTestContext _conferenceTestContext;

        public CommonSteps(ApiTestContext apiTestContext, ConferenceTestContext conferenceTestContext) : base(
            apiTestContext)
        {
            _apiTestContext = apiTestContext;
            _conferenceTestContext = conferenceTestContext;
        }

        [Given(@"I have a conference")]
        public async Task GivenIHaveAConference()
        {
            var seededConference = await ApiTestContext.TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            ApiTestContext.NewConferenceId = seededConference.Id;
            _conferenceTestContext.SeededConference = seededConference;
        }

        [When(@"I send the request to the endpoint")]
        [When(@"I send the same request twice")]
        public async Task WhenISendTheRequestToTheEndpoint()
        {
            _apiTestContext.ResponseMessage = new HttpResponseMessage();
            switch (_apiTestContext.HttpMethod.Method)
            {
                case "GET":
                    _apiTestContext.ResponseMessage = await SendGetRequestAsync(_apiTestContext);
                    break;
                case "POST":
                    _apiTestContext.ResponseMessage = await SendPostRequestAsync(_apiTestContext);
                    break;
                case "PATCH":
                    _apiTestContext.ResponseMessage = await SendPatchRequestAsync(_apiTestContext);
                    break;
                case "PUT":
                    _apiTestContext.ResponseMessage = await SendPutRequestAsync(_apiTestContext);
                    break;
                case "DELETE":
                    _apiTestContext.ResponseMessage = await SendDeleteRequestAsync(_apiTestContext);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(_apiTestContext.HttpMethod.ToString(),
                        _apiTestContext.HttpMethod.ToString(), null);
            }
        }

        [Then(@"the response should have the status (.*) and success status (.*)")]
        public void ThenTheResponseShouldHaveStatus(HttpStatusCode statusCode, bool isSuccess)
        {
            _apiTestContext.ResponseMessage.StatusCode.Should().Be(statusCode);
            _apiTestContext.ResponseMessage.IsSuccessStatusCode.Should().Be(isSuccess);
            TestContext.WriteLine($"Status Code: {_apiTestContext.ResponseMessage.StatusCode}");
        }

        [Then(@"the response message should read '(.*)'")]
        [Then(@"the error response message should contain '(.*)'")]
        [Then(@"the error response message should also contain '(.*)'")]
        public async Task ThenTheResponseShouldContain(string errorMessage)
        {
            var messageString = await _apiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            messageString.Should().Contain(errorMessage);
        }
    }
}