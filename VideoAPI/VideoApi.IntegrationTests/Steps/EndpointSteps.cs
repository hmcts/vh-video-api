using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AcceptanceTests.Common.Api.Helpers;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.IntegrationTests.Contexts;
using VideoApi.IntegrationTests.Helper;
using static Testing.Common.Helper.ApiUriFactory.EPEndpoints;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public class EndpointSteps : BaseSteps
    {
        private readonly TestContext _context;
        private readonly CommonSteps _commonSteps;
        
        public EndpointSteps(TestContext context, CommonSteps commonSteps)
        {
            _context = context;
            _commonSteps = commonSteps;
        }
        
        [Given(@"I have a get endpoints request for a nonexistent conference")]
        public void GivenIHaveARequestForEndpointsForNonexistentConference()
        {
            SetupGetEndpointRequest(Guid.NewGuid());
        }
        
        [Given(@"I have a conference with no endpoints")]
        public async Task GivenIHaveAConferenceWithNoEndpoints()
        {
            await _commonSteps.GivenIHaveAConference();
        }
        
        [Given(@"I have a conference with endpoints")]
        public async Task GivenIHaveAConferenceWithEndpoints()
        {
            var conference1 = new ConferenceBuilder()
                .WithEndpoint("Display1", "sip@123.com")
                .WithEndpoint("Display2", "sip@321.com").Build();
            _context.Test.Conference = await _context.TestDataManager.SeedConference(conference1);
            _context.Test.Conferences.Add(_context.Test.Conference);
            NUnit.Framework.TestContext.WriteLine($"New seeded conference id: {_context.Test.Conference.Id}");
        }
        
        [Given(@"I have get endpoints for conference request")]
        public void GivenIHaveAGetEndpointsForAConferenceRequest()
        {
            SetupGetEndpointRequest(_context.Test.Conference.Id);
        }
        
        [Given(@"I have an add endpoint to conference request")]
        public void GivenIHaveAValidAddEndpointRequest()
        {
            var conferenceId = _context.Test.Conference.Id;
            var request = new AddEndpointRequest
            {
                Pin = "1234",
                SipAddress = "1234add_auto_test@sip.com",
                DisplayName = "Automated Add EP test"
            };
            SetupAddEndpointRequest(conferenceId, request);
        }
        
        [Given(@"I have an add endpoint to a non-existent conference request")]
        public void GivenIHaveAnNonExistentAddEndpointRequest()
        {
            var conferenceId = Guid.NewGuid();
            var request = new AddEndpointRequest
            {
                Pin = "1234",
                SipAddress = "1234add_auto_test@sip.com",
                DisplayName = "Automated Add EP test"
            };
            SetupAddEndpointRequest(conferenceId, request);
        }
        
        [Given(@"I have an invalid add endpoint to conference request")]
        public void GivenIHaveAnInvalidAddEndpointRequest()
        {
            var conferenceId = Guid.NewGuid();
            var request = new AddEndpointRequest
            {
                SipAddress = string.Empty,
                DisplayName = string.Empty
            };
            SetupAddEndpointRequest(conferenceId, request);
        }
        
        [Then(@"the endpoint response should be (.*)")]
        public async Task ThenTheEndpointResponseShould(int number)
        {
            await AssertEndpointLength(number);
        }
        
        [Then(@"the endpoint response should not be empty")]
        public  async Task  ThenTheEndpointResponseShouldNotBeEmpty()
        {
            await AssertEndpointLength(_context.Test.Conference.Endpoints.Count);
        }
        
        [Then(@"the endpoint response should be empty")]
        public async Task ThenTheEndpointResponseShouldBeEmpty()
        {
            await AssertEndpointLength(0);
        }
        
        private async Task AssertEndpointLength(int length)
        {
            var result = await Response.GetResponses<IList<EndpointResponse>>(_context.Response.Content);
            result.Should().HaveCount(length);
        }

        private void SetupGetEndpointRequest(Guid conferenceId)
        {
            _context.Uri = GetEndpointsForConference(conferenceId);
            _context.HttpMethod = HttpMethod.Get;
        }
        
        private void SetupAddEndpointRequest(Guid conferenceId, AddEndpointRequest request)
        {
            var jsonBody = RequestHelper.SerialiseRequestToSnakeCaseJson(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            _context.Uri = AddEndpointsToConference(conferenceId);
            _context.HttpMethod = HttpMethod.Post;
        }
    }
}
