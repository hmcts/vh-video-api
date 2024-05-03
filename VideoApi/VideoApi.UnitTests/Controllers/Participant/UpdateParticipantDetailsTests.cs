using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Controllers.Participant
{
    public class UpdateParticipantDetailsTests : ParticipantsControllerTestBase
    {
        private UpdateParticipantRequest _updateParticipantRequest;

        [SetUp]
        public void TestInitialize()
        {
            _updateParticipantRequest = new UpdateParticipantRequest
            {
                Fullname = "Test Name",
                DisplayName = "Test N",
                Representee = "Represent"
            };

            MockQueryHandler
                .Setup(x =>
                    x.Handle<GetConferenceByIdQuery, VideoApi.Domain.Conference>(It.IsAny<GetConferenceByIdQuery>()))
                .ReturnsAsync(TestConference);
        }

        [Test]
        public async Task Should_update_given_participants_details()
        {
            var conferenceId = TestConference.Id;
            var participant = TestConference.GetParticipants()[1];


            var result = await Controller.UpdateParticipantDetailsAsync(conferenceId, participant.Id, _updateParticipantRequest);
            MockCommandHandler.Verify(c => c.Handle(It.IsAny<UpdateParticipantDetailsCommand>()), Times.Once);
            
            var typedResult = (NoContentResult)result;
            typedResult.Should().NotBeNull();
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_conference()
        {
            MockCommandHandler
                .Setup(
                    x => x.Handle(It.IsAny<UpdateParticipantDetailsCommand>()))
                .ThrowsAsync(new ConferenceNotFoundException(TestConference.Id));

            var result =
                await Controller.UpdateParticipantDetailsAsync(Guid.NewGuid(), Guid.NewGuid(),
                    _updateParticipantRequest);

            var typedResult = (NotFoundResult) result;
            typedResult.Should().NotBeNull();
        }

        [Test]
        public async Task Should_return_notfound_with_no_matching_participant()
        {
            MockCommandHandler
                .Setup(
                    x => x.Handle(It.IsAny<UpdateParticipantDetailsCommand>()))
                .ThrowsAsync(new ParticipantNotFoundException(Guid.NewGuid()));


            var result =
                await Controller.UpdateParticipantDetailsAsync(TestConference.Id, Guid.NewGuid(),
                    _updateParticipantRequest);

            var typedResult = (NotFoundResult) result;
            typedResult.Should().NotBeNull();
        }
        
        

        [Test]
        public async Task should_add_username_to_command_when_provided()
        {
            var conferenceId = TestConference.Id;
            var participant = TestConference.GetParticipants()[1];
            _updateParticipantRequest.Username = "new.me@hmcts.net;";


            var result = await Controller.UpdateParticipantDetailsAsync(conferenceId, participant.Id, _updateParticipantRequest);
            MockCommandHandler.Verify(c => c.Handle(It.Is<UpdateParticipantDetailsCommand>(c => c.Username == _updateParticipantRequest.Username)), Times.Once);
            
            var typedResult = (NoContentResult)result;
            typedResult.Should().NotBeNull();
        }
    }
}
