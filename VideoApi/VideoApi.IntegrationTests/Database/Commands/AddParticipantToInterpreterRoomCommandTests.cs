using System;
using System.Linq;
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
    public class AddParticipantToInterpreterRoomCommandTests : DatabaseTestsBase
    {
        private AddParticipantToInterpreterRoomCommandHandler _handler;
        private Guid _newConferenceId;
        
        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new AddParticipantToInterpreterRoomCommandHandler(context);
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
        public async Task should_add_participant_to_interpreter_room()
        {
            var conference = await TestDataManager.SeedConference();
            _newConferenceId = conference.Id;
            var interpreterRoom = new InterpreterRoom(_newConferenceId, "InterpreterRoom1", VirtualCourtRoomType.Witness);
            var rooms = await TestDataManager.SeedRooms(new[] {interpreterRoom});
            interpreterRoom = (InterpreterRoom) rooms[0];
            var participant = conference.Participants.First(x => x.UserRole == UserRole.Individual);

            var command = new AddParticipantToInterpreterRoomCommand(interpreterRoom.Id, participant.Id);
            await _handler.Handle(command);
            
            await using var db = new VideoApiDbContext(VideoBookingsDbContextOptions);
            var updatedRoom = await db.Rooms.OfType<InterpreterRoom>().AsNoTracking()
                .Include(r => r.RoomParticipants)
                .SingleAsync(r => r.Id == interpreterRoom.Id);

            updatedRoom.RoomParticipants.Any(p => p.ParticipantId == participant.Id).Should().BeTrue();
        }
        
        [Test]
        public void Should_throw_exception_if_no_room_found()
        {
            var command = new AddParticipantToInterpreterRoomCommand(999, Guid.NewGuid());
            Assert.ThrowsAsync<RoomNotFoundException>(() => _handler.Handle(command));
        }
    }
}
