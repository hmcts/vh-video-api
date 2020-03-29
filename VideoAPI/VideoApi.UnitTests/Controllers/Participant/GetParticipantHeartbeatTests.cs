using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Participant
{
    public class GetParticipantHeartbeatTests : ParticipantsControllerTestBase
    {
        [SetUp]
        public void TestInitialize()
        {
            MockQueryHandler
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);
            
            var heartbeats = new List<Heartbeat>
            {
                new Heartbeat(TestConference.Id, TestConference.Participants.First().Id, 1,2,3,4,5,6,7,8, DateTime.MaxValue, "chrome", "1")
            };

            MockQueryHandler
                .Setup(x => x.Handle<GetHeartbeatsFromTimePointQuery, IList<Heartbeat>>(It.IsAny<GetHeartbeatsFromTimePointQuery>()))
                .ReturnsAsync(heartbeats);
        }

        [Test]
        public async Task Should_get_heartbeatResponses()
        {
            var conferenceId = TestConference.Id;
            var participantId = TestConference.GetParticipants()[1].Id;
            

            var result = await Controller.GetHeartbeatDataForParticipantAsync(conferenceId, participantId);

            MockQueryHandler.Verify(m => m.Handle<GetHeartbeatsFromTimePointQuery, IList<Heartbeat>>(It.IsAny<GetHeartbeatsFromTimePointQuery>()), Times.Once);

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<OkObjectResult>();
            var responses = result.As<OkObjectResult>().Value.As<IEnumerable<ParticipantHeartbeatResponse>>().ToList();
            responses.Should().NotBeNull().And.NotBeEmpty().And.NotContainNulls();
            responses.First().As<ParticipantHeartbeatResponse>().BrowserName.Should().Be("chrome");
            responses.First().As<ParticipantHeartbeatResponse>().BrowserVersion.Should().Be("1");
            responses.First().As<ParticipantHeartbeatResponse>().RecentPacketLoss.Should().Be(8);
        }
    }
}
