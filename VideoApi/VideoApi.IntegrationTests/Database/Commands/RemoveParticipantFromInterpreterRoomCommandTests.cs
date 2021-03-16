using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class RemoveParticipantFromInterpreterRoomCommandTests : DatabaseTestsBase
    {
        private RemoveParticipantFromInterpreterRoomCommandHandler _handler;
        private Guid _newConferenceId;
        private InterpreterRoom _room;
        
        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new RemoveParticipantFromInterpreterRoomCommandHandler(context);
        }
        
        [TearDown]
        public async Task TearDown()
        {
            if (_newConferenceId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test room for conference {_newConferenceId}");
                await TestDataManager.RemoveRooms(_newConferenceId);

                TestContext.WriteLine($"Removing test conference {_newConferenceId}");
                await TestDataManager.RemoveConference(_newConferenceId);
                _newConferenceId = Guid.Empty;
            }
        }
        
        [Test]
        public void Should_throw_exception_if_no_room_found()
        {
            var command = new RemoveParticipantFromInterpreterRoomCommand(999, Guid.NewGuid());
            Assert.ThrowsAsync<RoomNotFoundException>(() => _handler.Handle(command));
        }
        
        [Test]
        public async Task should_remove_participant_from_interpreter_room()
        {
            var conference = await TestDataManager.SeedConference();
            _newConferenceId = conference.Id;
            await SetupRoomWithParticipant(conference);
            
            var participant = _room.RoomParticipants.First();

            var command = new RemoveParticipantFromInterpreterRoomCommand(_room.Id, participant.ParticipantId);
            await _handler.Handle(command);

            var updatedRoom = await GetInterpreterRoom();

            updatedRoom.RoomParticipants.Any(p => p.ParticipantId == participant.ParticipantId).Should().BeFalse();
        }

        private async Task SetupRoomWithParticipant(Conference conference)
        {
            var interpreterRoom = new InterpreterRoom(_newConferenceId, "InterpreterRoom1", VirtualCourtRoomType.Witness);
            var rooms = await TestDataManager.SeedRooms(new[] {interpreterRoom});
            _room = (InterpreterRoom) rooms[0];
            var participant = conference.Participants.First(x => x.UserRole == UserRole.Individual);
            
            await using var db = new VideoApiDbContext(VideoBookingsDbContextOptions);
            var room = await db.Rooms.OfType<InterpreterRoom>()
                .Include(r => r.RoomParticipants)
                .SingleAsync(r => r.Id == _room.Id);
            room.AddParticipant(new RoomParticipant(participant.Id));
            await db.SaveChangesAsync();
            _room = room;
        }

        private async Task<InterpreterRoom> GetInterpreterRoom()
        {
            await using var db = new VideoApiDbContext(VideoBookingsDbContextOptions);
            return await db.Rooms.OfType<InterpreterRoom>()
                .Include(r => r.RoomParticipants)
                .SingleAsync(r => r.Id == _room.Id);
        }
    }
}
