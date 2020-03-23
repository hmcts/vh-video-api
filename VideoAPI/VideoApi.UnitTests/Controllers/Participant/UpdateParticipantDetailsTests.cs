using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using System;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Participant
{
    public class UpdateParticipantDetailsTests : ParticipantsControllerTestBase
    {
        private UpdateParticipantRequest updateParticipantRequest;

        [SetUp]
        public void TestInitialize()
        {
            updateParticipantRequest = new UpdateParticipantRequest { 
                                                Fullname = "Test Name", 
                                                DisplayName = "Test N", 
                                                Representee = "Represent" };

            _mockQueryHandler
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);
        }

        [Test]
        public async Task Should_update_given_participants_details()
        {
            var conferenceId = TestConference.Id;
            var participant = TestConference.GetParticipants()[1];
            

            await _controller.UpdateParticipantDetailsAsync(conferenceId, participant.Id, updateParticipantRequest);

            _mockQueryHandler.Verify(m => m.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()), Times.Once);
            _mockCommandHandler.Verify(c => c.Handle(It.IsAny<UpdateParticipantDetailsCommand>()), Times.Once);
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_conference()
        {
            _mockQueryHandler
                .Setup(x => x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync((VideoApi.Domain.Conference)null); 
            
            var result = await _controller.UpdateParticipantDetailsAsync(Guid.NewGuid(), Guid.NewGuid(), updateParticipantRequest);

            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_participant()
        {
            var result = await _controller.UpdateParticipantDetailsAsync(TestConference.Id, Guid.NewGuid(), updateParticipantRequest);

            var typedResult = (NotFoundResult)result;
            typedResult.Should().NotBeNull();
        }
    }
}
