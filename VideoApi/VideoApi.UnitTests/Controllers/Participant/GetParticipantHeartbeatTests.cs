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
                new Heartbeat(TestConference.Id, TestConference.Participants.First().Id, 1, 2, 3, 4, 5, 6, 7, 8,
                    DateTime.MaxValue, "chrome", "1", "Mac OS X", "10.15.7",0,"25kbps","opus",1,1,0,25,"2kbps","H264","640x480","18kbps","opus",1,0,"106kbps","VP8","1280x720",1,0 )
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
            var heartbeatResponse = responses.First().As<ParticipantHeartbeatResponse>();
            heartbeatResponse.BrowserName.Should().Be("chrome");
            heartbeatResponse.BrowserVersion.Should().Be("1");
            heartbeatResponse.RecentPacketLoss.Should().Be(8);
            heartbeatResponse.OperatingSystem.Should().Be("Mac OS X");
            heartbeatResponse.OperatingSystemVersion.Should().Be("10.15.7");
        }
    }
}
