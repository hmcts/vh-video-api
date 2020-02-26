using System;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Participant
{
    public class SaveParticipantHeartbeatTests : ParticipantsControllerTestBase
    {
        [SetUp]
        public void TestInitialize()
        {
            _mockQueryHandler
                .Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);
        }

        [Test]
        public async Task Should_save_heartbeat()
        {
            var conferenceId = TestConference.Id;
            var participantId = TestConference.GetParticipants()[1].Id;
            
            var result = await _controller.SaveHeartbeatDataForParticipantAsync(conferenceId, participantId, new AddHeartbeatRequest());

            _mockQueryHandler.Verify(m => m.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()), Times.Once);
            _mockCommandHandler.Verify(c => c.Handle(It.IsAny<SaveHeartbeatCommand>()), Times.Once);

            result.Should().NotBeNull();
            result.Should().BeAssignableTo<NoContentResult>();
        }

        [Test]
        public async Task Should_return_badrequest_when_request_is_null()
        {
            _mockQueryHandler
                .Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync((Conference)null); 
            
            var result = await _controller.SaveHeartbeatDataForParticipantAsync(Guid.Empty, Guid.Empty, null);

            var typedResult = (BadRequestResult)result;
            typedResult.Should().NotBeNull();
        }
        
        [Test]
        public async Task Should_return_notfound_with_no_matching_conference()
        {
            _mockQueryHandler
                .Setup(x => x.Handle<GetConferenceByIdQuery, Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync((Conference)null); 
            
            var result = await _controller.SaveHeartbeatDataForParticipantAsync(Guid.Empty, Guid.Empty, new AddHeartbeatRequest());

            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_participant()
        {
            var result = await _controller.SaveHeartbeatDataForParticipantAsync(Guid.Empty, Guid.Empty, new AddHeartbeatRequest());

            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }
    }
}
