using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;

namespace VideoApi.UnitTests.Controllers.Participant
{
    public class UpdateConferenceParticipantsTests : ParticipantsControllerTestBase
    {
        private IList<UpdateParticipantRequest> _existingParticipants;
        private IList<ParticipantRequest> _newParticipants;
        private IList<Guid> _removedParticipants;
        private IList<LinkedParticipantRequest> _linkedParticipants;
        private UpdateConferenceParticipantsRequest _request;

        [SetUp]
        public void SetUp()
        {
            _existingParticipants = new List<UpdateParticipantRequest>
            {
                new()
                {
                    ContactEmail = "email@phoneNumber.com",
                    DisplayName = "Displayname",
                    ParticipantRefId = Guid.NewGuid()
                }
            };

            _newParticipants = new List<ParticipantRequest>
            {
                new()
                {
                    ContactEmail = "email@phoneNumber.com",
                    DisplayName = "Displayname",
                    HearingRole = "HearingRole",
                    ParticipantRefId = Guid.NewGuid(),
                    Username = "Username",
                    UserRole = UserRole.Individual
                }
            };

            _removedParticipants = new List<Guid> { Guid.NewGuid() };

            _linkedParticipants = new List<LinkedParticipantRequest>
            {
                new()
                {
                    LinkedRefId = Guid.NewGuid(),
                    ParticipantRefId = Guid.NewGuid(),
                    Type = LinkedParticipantType.Interpreter
                }
            };

            _request = new UpdateConferenceParticipantsRequest
            {
                ExistingParticipants = _existingParticipants,
                LinkedParticipants = _linkedParticipants,
                NewParticipants = _newParticipants,
                RemovedParticipants = _removedParticipants
            };
        }

        [Test]
        public async Task Should_call_command_to_update_conference_participants_and_return_no_content_response()
        {
            //Arrange/Act
            var result = await Controller.UpdateConferenceParticipantsAsync(TestConference.Id, _request);

            //Assert
            MockCommandHandler.Verify(c => c.Handle(It.IsAny<UpdateConferenceParticipantsCommand>()), Times.Once);

            result.Should().NotBeNull();
            result.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public async Task Should_return_conference_not_found_exception_when_conference_is_not_found()
        {
            //Arrange
            MockCommandHandler
                .Setup(
                    x => x.Handle(It.IsAny<UpdateConferenceParticipantsCommand>()))
                .ThrowsAsync(new ConferenceNotFoundException(TestConference.Id));

            //Act
            var result = await Controller.UpdateConferenceParticipantsAsync(Guid.NewGuid(), _request);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Test]
        public async Task Should_return_participant_not_found_exception_when_participant_is_not_found()
        {
            //Arrange
            MockCommandHandler
                .Setup(
                    x => x.Handle(It.IsAny<UpdateConferenceParticipantsCommand>()))
                .ThrowsAsync(new ParticipantNotFoundException(Guid.NewGuid()));

            //Act
            var result = await Controller.UpdateConferenceParticipantsAsync(Guid.NewGuid(), _request);

            //Asssert
            result.Should().NotBeNull();
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
