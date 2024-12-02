using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class UpdateParticipantStatusAndRoomCommandTests : DatabaseTestsBase
    {
        private UpdateParticipantStatusAndRoomCommandHandler _handler;
        private GetConferenceByIdQueryHandler _conferenceByIdHandler;
        private List<Guid> _newConferenceIds;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new UpdateParticipantStatusAndRoomCommandHandler(context);
            _conferenceByIdHandler = new GetConferenceByIdQueryHandler(context);
            _newConferenceIds = new List<Guid>();
        }

        [Test]
        public void Should_throw_conference_not_found_exception_when_conference_does_not_exist()
        {
            var conferenceId = Guid.NewGuid();
            var participantId = Guid.NewGuid();
            var state = ParticipantState.InConsultation;
            var room = RoomType.ConsultationRoom;

            var command = new UpdateParticipantStatusAndRoomCommand(conferenceId, participantId, state, room, null);

            Assert.ThrowsAsync<ConferenceNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public async Task Should_throw_participant_not_found_exception_when_participant_does_not_exist()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceIds.Add(seededConference.Id);
            var participantId = Guid.NewGuid();
            var state = ParticipantState.InConsultation;
            var room = RoomType.ConsultationRoom;

            var command = new UpdateParticipantStatusAndRoomCommand(seededConference.Id, participantId, state, room, null);

            Assert.ThrowsAsync<ParticipantNotFoundException>(() => _handler.Handle(command));
        }

        [Test] public async Task should_throw_exception_if_updating_to_consultaiton_room_not_created_first()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceIds.Add(seededConference.Id);
            var participant = seededConference.GetParticipants().First();
            const ParticipantState state = ParticipantState.InConsultation;
            const string staticRoomlabel = "ConsultationRoom1";

            var command = new UpdateParticipantStatusAndRoomCommand(seededConference.Id, participant.Id, state, null, staticRoomlabel);
            Assert.ThrowsAsync<RoomNotFoundException>(() => _handler.Handle(command));
        }


        [Test]
        public async Task should_not_throw_room_not_found_exception_when_updating_participant_to_InConsultation()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceIds.Add(seededConference.Id);
            var participant = seededConference.GetParticipants().First();
            const ParticipantState state = ParticipantState.Disconnected;

            var command = new UpdateParticipantStatusAndRoomCommand(seededConference.Id, participant.Id, state, null, null);
            await _handler.Handle(command);
            
            var updatedConference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(seededConference.Id));
            var updatedParticipant = updatedConference.GetParticipants().Single(x => x.Username == participant.Username);
            var afterState = updatedParticipant.GetCurrentStatus();
            afterState.ParticipantState.Should().Be(state);
        }

        [Test]
        public async Task Should_update_participant_status_to_in_consultation_in_consultation_room_1()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceIds.Add(seededConference.Id);
            var participant = seededConference.GetParticipants().First();
            const ParticipantState state = ParticipantState.InConsultation;
            const RoomType room = RoomType.ConsultationRoom;

            var beforeState = participant.GetCurrentStatus();

            var command = new UpdateParticipantStatusAndRoomCommand(seededConference.Id, participant.Id, state, room, null);
            await _handler.Handle(command);

            var updatedConference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(seededConference.Id));
            var updatedParticipant = updatedConference.GetParticipants().Single(x => x.Username == participant.Username);
            var afterState = updatedParticipant.GetCurrentStatus();

            afterState.Should().NotBe(beforeState);
            afterState.ParticipantState.Should().Be(state);
            updatedParticipant.CurrentRoom.Should().Be(room);
        }

        [Test]
        public async Task Should_update_participant_status_and_virtual_room()
        {
            var seededConference = await TestDataManager.SeedConference();
            _newConferenceIds.Add(seededConference.Id);
            var consultationRoom = new ConsultationRoom(seededConference.Id, $"JudgeConsultationRoom{DateTime.UtcNow.Ticks}",
                VirtualCourtRoomType.JudgeJOH, false);
            var seededRoom = await TestDataManager.SeedRoom(consultationRoom);
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            TestContext.WriteLine($"New seeded room id: {seededRoom.Id}");
            var participant = seededConference.GetParticipants().First(p => p is Participant && ((Participant)p).IsJudge());
            const ParticipantState state = ParticipantState.InConsultation;

            var beforeState = participant.GetCurrentStatus();

            var command = new UpdateParticipantStatusAndRoomCommand(seededConference.Id, participant.Id, state, null, seededRoom.Label);
            await _handler.Handle(command);

            var updatedConference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(seededConference.Id));
            var updatedParticipant = updatedConference.GetParticipants().Single(x => x.Username == participant.Username);
            var afterState = updatedParticipant.GetCurrentStatus();

            afterState.Should().NotBe(beforeState);
            afterState.ParticipantState.Should().Be(state);
            updatedParticipant.CurrentRoom.Should().BeNull();
            updatedParticipant.CurrentConsultationRoom.Label.Should().Be(seededRoom.Label);
        }


        [Test]
        public async Task should_update_participant_to_disconnected_from_virtual_room()
        {
            // Arrange conference with participant in consultation room
            var seededConference = await TestDataManager.SeedConference();
            _newConferenceIds.Add(seededConference.Id);
            var consultationRoom = new ConsultationRoom(seededConference.Id, $"JudgeConsultationRoom{DateTime.UtcNow.Ticks}",
                VirtualCourtRoomType.JudgeJOH, false);
            
            var room = await TestDataManager.SeedRoom(consultationRoom);
            var newRoomId = room.Id;

            var pat1 = seededConference.Participants[0].Id;
            await TestDataManager.SeedRoomWithRoomParticipant(newRoomId, new RoomParticipant(pat1));

            // Act
            var command =
                new UpdateParticipantStatusAndRoomCommand(seededConference.Id, pat1, ParticipantState.Disconnected, null,
                    null);
            await _handler.Handle(command);

            // Assert
            var updatedConference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(seededConference.Id));
            var updatedParticipant = updatedConference.GetParticipants().Single(x => x.Id == pat1);
            updatedParticipant.State.Should().Be(ParticipantState.Disconnected);
            updatedParticipant.CurrentRoom.Should().BeNull();
            updatedParticipant.CurrentConsultationRoom.Should().BeNull();
        }
        
        [TearDown]
        public async Task TearDownAsync()
        {
            foreach (var conferenceId in _newConferenceIds)
            {
                TestContext.WriteLine($"Removing test conference {conferenceId}");
                await TestDataManager.RemoveConference(conferenceId);

                TestContext.WriteLine("Cleaning rooms for GetAvailableRoomByRoomTypeQuery");
                await TestDataManager.RemoveRooms(conferenceId);
            }
        }
    }
}
