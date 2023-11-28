using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Testing.Common.Assertions;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Queries;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Participant
{
    public class SaveParticipantHeartbeatTests : ParticipantsControllerTestBase
    {
        [SetUp]
        public void TestInitialize()
        {
            MockQueryHandler
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);
        }

        [Test]
        public async Task Should_save_heartbeat()
        {
            var conferenceId = TestConference.Id;
            var participantId = TestConference.GetParticipants()[1].Id;
            
            var result = await Controller.SaveHeartbeatDataForParticipantAsync(conferenceId, participantId, new AddHeartbeatRequest());

            MockCommandHandler.Verify(c => c.Handle(It.IsAny<SaveHeartbeatCommand>()), Times.Once);

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<NoContentResult>();
        }

        [Test]
        public async Task Should_return_badrequest_when_request_is_null()
        {
            MockQueryHandler
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync((VideoApi.Domain.Conference)null); 
            
            var result = await Controller.SaveHeartbeatDataForParticipantAsync(Guid.Empty, Guid.Empty, null);

            var typedResult = (ObjectResult)result;
            typedResult.Should().NotBeNull();
            typedResult.ContainsValidationErrors();
        }
    }
}
