using System;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
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
    public class AddParticipantToInterpreterRoomCommandTests : DatabaseTestsBase
    {
        private AddParticipantToParticipantRoomCommandHandler _handler;
        private GetConferenceByIdQueryHandler _conferenceByIdHandler;
        private Guid _newConferenceId;
        
        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new AddParticipantToParticipantRoomCommandHandler(context);
            _conferenceByIdHandler = new GetConferenceByIdQueryHandler(context);
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
            var interpreterRoom = new ParticipantRoom(_newConferenceId, "InterpreterRoom1", VirtualCourtRoomType.Witness);
            var rooms = await TestDataManager.SeedRooms(new[] {interpreterRoom});
            interpreterRoom = (ParticipantRoom) rooms[0];
            var participant = conference.Participants.First(x => x.UserRole == UserRole.Individual);

            var command = new AddParticipantToParticipantRoomCommand(interpreterRoom.Id, participant.Id);
            await _handler.Handle(command);
            
            await using var db = new VideoApiDbContext(VideoBookingsDbContextOptions);
            var updatedRoom = await db.Rooms.AsQueryable().OfType<ParticipantRoom>().AsNoTracking()
                .Include(r => r.RoomParticipants)
                .SingleAsync(r => r.Id == interpreterRoom.Id);

            updatedRoom.RoomParticipants.Any(p => p.ParticipantId == participant.Id).Should().BeTrue();

            var updatedParticipantFromDb = await db.Participants.Include(x => x.RoomParticipants).ThenInclude(x=> x.Room).AsNoTracking()
                .SingleAsync(p => p.Id == participant.Id);

            updatedParticipantFromDb.RoomParticipants.Any(r => r.RoomId == interpreterRoom.Id).Should().BeTrue();
            
            var updatedConference = await _conferenceByIdHandler.Handle(new GetConferenceByIdQuery(_newConferenceId));
            var updatedParticipant = updatedConference.Participants.First(x => x.Id == participant.Id);
            updatedParticipant.GetParticipantRoom().Should().NotBeNull();
            updatedParticipant.GetParticipantRoom().Id.Should().Be(interpreterRoom.Id);
        }
        
        [Test]
        public void Should_throw_exception_if_no_room_found()
        {
            var command = new AddParticipantToParticipantRoomCommand(999, Guid.NewGuid());
            Assert.ThrowsAsync<RoomNotFoundException>(() => _handler.Handle(command));
        }
    }
}
