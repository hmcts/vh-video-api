using System;
using System.Collections.Generic;
using System.Net.Http;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using VideoApi.Common.Helpers;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.IntegrationTests.Contexts;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public class HeartbeatSteps : StepsBase
    {
        private readonly ConferenceTestContext _conferenceTestContext;
        private readonly HeartbeatEndpoints _endpoints = new ApiUriFactory().HeartbeatEndpoints;

        public HeartbeatSteps(ApiTestContext apiTestContext, ConferenceTestContext conferenceTestContext) : base(apiTestContext)
        {
            _conferenceTestContext = conferenceTestContext;
        }

        [Given(@"I have heartbeats")]
        public async Task GivenIHaveHeartbeats()
        {
            var conferenceId = _conferenceTestContext.SeededConference.Id;
            var participantId = _conferenceTestContext.SeededConference.GetParticipants()[0].Id;
            
            var heartbeats = new List<Heartbeat>
            {
                new Heartbeat(conferenceId, participantId, 1,2,3,4,5,6,7,8, DateTime.UtcNow.AddMinutes(5), "chrome", "1"),
                new Heartbeat(conferenceId, participantId, 8,7,6,5,4,3,2,1, DateTime.UtcNow.AddMinutes(2), "chrome", "1"),
                new Heartbeat(conferenceId, participantId, 5456,4495,5642,9795,5653,8723,4242,3343, DateTime.UtcNow.AddMinutes(1), "chrome", "1")
            };

            await ApiTestContext.TestDataManager.SeedHeartbeats(heartbeats);
            
            ApiTestContext.Uri = _endpoints.GetHeartbeats(conferenceId, participantId);
            ApiTestContext.HttpMethod = HttpMethod.Get;
        }
        
        [Then(@"(.*) heartbeats should be retrieved")]
        public async Task ThenTheChatMessagesShouldBeRetrieved(int count)
        {
            var json = await ApiTestContext.ResponseMessage.Content.ReadAsStringAsync();
            var result = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<List<ParticipantHeartbeatResponse>>(json);
            result.Should().NotBeNullOrEmpty().And.NotContainNulls();
            result.Should().HaveCount(count);
            result.Should().BeInAscendingOrder(x => x.Timestamp);
            result.Should().ContainItemsAssignableTo<ParticipantHeartbeatResponse>();
            result.Should().OnlyContain(x => x.BrowserName == "chrome");
            result.Should().OnlyContain(x => x.BrowserVersion == "1");
            Assert.AreEqual(9795, result[0].RecentPacketLoss);
            Assert.AreEqual(7, result[1].RecentPacketLoss);
            Assert.AreEqual(8, result[2].RecentPacketLoss);
        }
    }
}
