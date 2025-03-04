using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.DTOs;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class UpdateConferenceParticipantsCommandTests : DatabaseTestsBase
    {
        private UpdateConferencParticipantsCommandHandler _handler;
        private GetConferenceByIdQueryHandler _conferenceByIdHandler;

        private Conference _conference;

        private List<ParticipantBase> ExistingParticipants { get; set; }
        private List<ParticipantBase> NewParticipants { get; set; }
        private List<Guid> RemovedParticipantIds { get; set; }
        private List<LinkedParticipantDto> LinkedParticipants { get; set; }

        [SetUp]
        public async Task Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new UpdateConferencParticipantsCommandHandler(context);
            _conferenceByIdHandler = new GetConferenceByIdQueryHandler(context);

            ExistingParticipants = [];
            NewParticipants = [];
            RemovedParticipantIds = [];
            LinkedParticipants = [];

            _conference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {_conference.Id}");
        }

        [Test]
        public void Should_throw_conference_not_found_exception_when_conference_does_not_exist()
        {
            //Arrange
            var command = BuildCommand();
            command.ConferenceId = Guid.Empty;

            //Act/Assert
            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public async Task Should_add_participants_to_conference()
        {
            //Arrange
            var originalParticipantCount = _conference.GetParticipants().Count;

            var participantOne = new ParticipantBuilder(true).Build();
            var participantTwo = new ParticipantBuilder(true).Build();
            participantTwo.Username = "participantTwoUsername@participant.com";

            NewParticipants = new List<ParticipantBase>() { participantOne, participantTwo };

            var command = BuildCommand();

            //Act
            await _handler.Handle(command);

            var conference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_conference.Id));
            var participants = conference.GetParticipants();
            var newParticipantCount = conference.GetParticipants().Count;

            //Assert
            participants.Any(x => x.Username == participantOne.Username).Should().BeTrue();
            participants.Any(x => x.Username == participantTwo.Username).Should().BeTrue();
            newParticipantCount.Should().Be(originalParticipantCount + 2);
        }

        [Test]
        public void Should_throw_participant_not_found_exception_if_a_participant_cannot_be_found_when_updating()
        {
            //Arrange
            var participants = _conference.Participants.ToList();

            foreach (var participant in participants)
            {
                _conference.RemoveParticipant(participant);
            }

            var participantOne = new ParticipantBuilder(true).Build();

            ExistingParticipants.Add(participantOne);

            var command = BuildCommand();

            //Act/Assert
            Assert.ThrowsAsync<ParticipantNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public async Task Should_update_existing_participants_in_conference_and_remove_any_linked_participants_they_have()
        {
            //Arrange
            var seedConference = new ConferenceBuilder(true)
                .WithLinkedParticipant(UserRole.Individual, "Applicant")
                .Build();
            
            _conference = await TestDataManager.SeedConference(seedConference);

            var participantOne = _conference.Participants[0];
            var participantOnesLinkedParticipant = _conference.Participants[1];

            participantOne.DisplayName = "UpdatedDisplayName";
            participantOne.Username = "UpdatedUsername@username.com";

            if (participantOne is Participant participantOneCasted)
            {
                participantOneCasted.ContactEmail = "hi@dontcontactme.com";
            }

            ExistingParticipants.Add(participantOne);
            ExistingParticipants.Add(participantOnesLinkedParticipant);
            var command = BuildCommand();

            //Act
            await _handler.Handle(command);
            var conference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_conference.Id));
            var updatedParticipant = conference.Participants.SingleOrDefault(x => x.Id == participantOne.Id);
            var updatedParticipantsLinkedParticipant = conference.Participants.SingleOrDefault(x => x.Id == participantOnesLinkedParticipant.Id);

            //Assert
            updatedParticipant.DisplayName.Should().Be(participantOne.DisplayName);
            updatedParticipant.Username.Should().Be(participantOne.Username);
            updatedParticipant.LinkedParticipants.Should().BeEmpty();
            updatedParticipantsLinkedParticipant.LinkedParticipants.Should().BeEmpty();

            if (participantOne is Participant participantCasted)
            {
                ((Participant)updatedParticipant).ContactEmail.Should().Be(participantCasted.ContactEmail);
            }
        }

        [Test]
        public void Should_throw_participant_not_found_exception_if_a_participant_cannot_be_found_when_removing()
        {
            //Arrange
            var participants = _conference.Participants.ToList();

            foreach (var participant in participants)
            {
                _conference.RemoveParticipant(participant);
            }

            var participantOne = new ParticipantBuilder(true).Build();

            RemovedParticipantIds.Add(participantOne.Id);

            var command = BuildCommand();

            //Act/Assert
            Assert.ThrowsAsync<ParticipantNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public void Should_throw_participant_link_exception_if_a_participant_cannot_be_found()
        {
            //Arrange
            var participants = _conference.Participants.ToList();

            foreach (var participant in participants)
            {
                _conference.RemoveParticipant(participant);
            }

            var participantOne = new ParticipantBuilder(true).Build();
            var participantTwo = new ParticipantBuilder(true).Build();

            LinkedParticipants.Add(new LinkedParticipantDto
            {
                ParticipantRefId = participantOne.ParticipantRefId,
                LinkedRefId = participantTwo.ParticipantRefId,
                Type = LinkedParticipantType.Interpreter
            });

            var command = BuildCommand();

            //Act/Assert
            Assert.ThrowsAsync<ParticipantLinkException>(() => _handler.Handle(command));
        }

        [Test]
        public async Task Should_remove_participants_from_conference()
        {
            //Arrange
            var removedParticipantId = _conference.Participants[0].ParticipantRefId;
            RemovedParticipantIds = new List<Guid>() { removedParticipantId };
            var command = BuildCommand();

            //Act
            await _handler.Handle(command);

            var conference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_conference.Id));
            var participants = conference.GetParticipants();

            //Assert
            participants.SingleOrDefault(x => x.Id == removedParticipantId).Should().BeNull();
        }

        [Test]
        public async Task Should_add_linked_participants_to_conference()
        {
            //Arrange
            var participantOne = _conference.Participants[0];
            var participantTwo = _conference.Participants[1];

            LinkedParticipants.Add(new LinkedParticipantDto
            {
                ParticipantRefId = participantOne.ParticipantRefId,
                LinkedRefId = participantTwo.ParticipantRefId,
                Type = LinkedParticipantType.Interpreter
            });

            var command = BuildCommand();

            //Act
            await _handler.Handle(command);

            var conference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_conference.Id));
            var participants = conference.GetParticipants();

            participantOne = participants.SingleOrDefault(x => x.Username == participantOne.Username);
            participantTwo = participants.SingleOrDefault(x => x.Username == participantTwo.Username);

            //Assert
            participantOne.LinkedParticipants[0].ParticipantId.Should().Be(participantOne.Id);
            participantOne.LinkedParticipants[0].LinkedId.Should().Be(participantTwo.Id);
            participantOne.LinkedParticipants[0].Type.Should().Be(LinkedParticipantType.Interpreter);

            participantTwo.LinkedParticipants[0].ParticipantId.Should().Be(participantTwo.Id);
            participantTwo.LinkedParticipants[0].LinkedId.Should().Be(participantOne.Id);
            participantTwo.LinkedParticipants[0].Type.Should().Be(LinkedParticipantType.Interpreter);
        }

        [Test]
        public async Task Should_remove_a_participant_then_add_with_another_hearing_role()
        {
            // Arrange
            var participantOne = _conference.Participants[0];
            var participantTwo = _conference.Participants[1];

            ExistingParticipants.Add(participantOne);
            ExistingParticipants.Add(participantTwo);

            var removedParticipantId = participantOne.ParticipantRefId;
            RemovedParticipantIds = new List<Guid>() { removedParticipantId };

            var updatedParticipant = new ParticipantBuilder(true).Build();
            updatedParticipant.ParticipantRefId = participantOne.ParticipantRefId;
            updatedParticipant.Username = participantOne.Username;
            updatedParticipant.UserRole = UserRole.Representative;

            NewParticipants.Add(updatedParticipant);

            var command = BuildCommand();

            // Act
            await _handler.Handle(command);
            var conference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_conference.Id));
            var participants = conference.GetParticipants();

            //Assert
            participants.Count.Should().Be(6);
            participants.SingleOrDefault(x => x.Id == removedParticipantId).Should().BeNull();
            participants.SingleOrDefault(x => x.Id == updatedParticipant.Id).Should().NotBeNull();
        }
        
        [Test]
        public async Task Should_remove_multiple_participants_then_add_participant_with_another_hearing_role()
        {
            // Arrange
            var participantOne = _conference.Participants[0];
            var participantTwo = _conference.Participants[1];

            ExistingParticipants.Add(participantOne);
            ExistingParticipants.Add(participantTwo);

            RemovedParticipantIds = new List<Guid>() { participantOne.ParticipantRefId, participantTwo.ParticipantRefId };

            var updatedParticipant1 = new ParticipantBuilder(true).Build();
            var updatedParticipant2 = new ParticipantBuilder(true).Build();

            updatedParticipant1.ParticipantRefId = participantOne.ParticipantRefId;
            updatedParticipant1.Username = participantOne.Username;
            updatedParticipant1.UserRole = UserRole.Representative;
            
            updatedParticipant2.ParticipantRefId = participantTwo.ParticipantRefId;
            updatedParticipant2.Username = participantTwo.Username;
            updatedParticipant2.UserRole = UserRole.Representative;

            NewParticipants.Add(updatedParticipant1);
            NewParticipants.Add(updatedParticipant2);

            var command = BuildCommand();

            // Act
            await _handler.Handle(command);
            var conference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_conference.Id));
            var participants = conference.GetParticipants();

            //Assert
            participants.Count.Should().Be(6);
            participants.SingleOrDefault(x => x.Id == participantOne.ParticipantRefId).Should().BeNull();
            participants.SingleOrDefault(x => x.Id == updatedParticipant1.Id).Should().NotBeNull();
            participants.SingleOrDefault(x => x.Id == updatedParticipant2.Id).Should().NotBeNull();
        }

        private UpdateConferenceParticipantsCommand BuildCommand()
        {
            return new UpdateConferenceParticipantsCommand(_conference.Id, ExistingParticipants, NewParticipants, RemovedParticipantIds, LinkedParticipants);
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_conference.Id != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test conference {_conference.Id}");
                await TestDataManager.RemoveConference(_conference.Id);
            }
        }
    }
}
