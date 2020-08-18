using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using TechTalk.SpecFlow;
using Testing.Common.Helper.Builders.Domain;
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
            SetupEndpointRequest(Guid.NewGuid());
        }
        
        [Given(@"I have a conference with no endpoints")]
        public async Task GivenIHaveAConferenceWithNoEndpoints()
        {
            await _commonSteps.GivenIHaveAConference();
            SetupEndpointRequest(_context.Test.Conference.Id);
        }
        
        [Given(@"I have a conference with endpoints")]
        public async Task GivenIHaveAConferenceWithEndpoints()
        {
            var conference1 = new ConferenceBuilder()
                .WithEndpoint("Display1", "sip@123.com")
                .WithEndpoint("Display2", "sip@321.com").Build();
            _context.Test.Conference = await _context.TestDataManager.SeedConference(conference1);
            NUnit.Framework.TestContext.WriteLine($"New seeded conference id: {_context.Test.Conference.Id}");

            SetupEndpointRequest(conference1.Id);
        }
        
        [Then(@"the endpoint response should not be empty")]
        public  async Task  ThenTheEndpointResponseShouldNotBeEmpty()
        {
            await AssertEndpointLength(_context.Test.Conference.Endpoints.Count);
        }
        
        [Then(@"the endpoint response should be empty")]
        public async Task ThenTheEndpointResponseShouldBeEmpty()
        {
            if (_context.Test.Conference == null)
            {
                await AssertEndpointLength(0);
            }
            else
            {
                await AssertEndpointLength(_context.Test.Conference.Endpoints.Count);
            }
        }
        
        private async Task AssertEndpointLength(int length)
        {
            var result = await Response.GetResponses<IList<EndpointResponse>>(_context.Response.Content);
            result.Should().HaveCount(length);
        }

        private void SetupEndpointRequest(Guid conferenceId)
        {
            _context.Uri = GetEndpointsForConference(conferenceId);
            _context.HttpMethod = HttpMethod.Get;
            
        }
    }
}
