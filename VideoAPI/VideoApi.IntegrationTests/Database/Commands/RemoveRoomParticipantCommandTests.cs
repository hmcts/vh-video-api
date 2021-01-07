using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.IntegrationTests.Helper;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class RemoveRoomParticipantCommandTests : DatabaseTestsBase
    {
        private RemoveRoomParticipantCommandHandler _handler;
        private Guid _newConferenceId;
        private long _newRoomId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new RemoveRoomParticipantCommandHandler(context);
        }

        [Test]
        public async Task Should_remove_participant_from_room()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;

            var participant = seededConference.Participants.FirstOrDefault(x => x.UserRole == UserRole.Judge);
            var room = await TestDataManager.SeedRoom(new Room(_newConferenceId, "Room1", VirtualCourtRoomType.JudgeJOH));
            _newRoomId = room.Id;

            var roomWithParticipant = await TestDataManager.SeedRoomWithRoomParticipant(_newRoomId, new RoomParticipant(_newRoomId, participant.Id, DateTime.UtcNow));

            var currentCount = roomWithParticipant.RoomParticipants.Count;
            currentCount.Should().BeGreaterThan(0);

            var command = new RemoveRoomParticipantCommand(participant.Id, _newRoomId);
            await _handler.Handle(command);

            Room roomSaved;
            await using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                roomSaved = await db.Rooms.Include("RoomParticipants")
                    .SingleOrDefaultAsync(x => x.Id == _newRoomId);
            }

            var savedRoomParticipantCount = roomSaved.RoomParticipants.Count;


            savedRoomParticipantCount.Should().BeLessThan(currentCount);
        }

        [Test]
        public void Should_throw_exception_if_no_room_found()
        {
            var command = new RemoveRoomParticipantCommand(Guid.NewGuid(), _newRoomId);
            Assert.ThrowsAsync<RoomNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public async Task Should_throw_exception_if_no_room_participant_found()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;

            var room = await TestDataManager.SeedRoom(new Room(_newConferenceId, "Room1", VirtualCourtRoomType.JudgeJOH));

            _newRoomId = room.Id;
            var command = new RemoveRoomParticipantCommand(Guid.NewGuid(), _newRoomId);
            Assert.ThrowsAsync<RoomParticipantNotFoundException>(() => _handler.Handle(command));
        }

        [TearDown]
        public async Task TearDown()
        {
            if (_newRoomId > 0)
            {
                TestContext.WriteLine($"Removing test room {_newRoomId} for conference {_newConferenceId}");
                await TestDataManager.RemoveRooms(_newConferenceId);
                _newRoomId = 0;
            }
            if (_newConferenceId != Guid.Empty)
            {
                TestContext.WriteLine($"Removing test conference {_newConferenceId}");
                await TestDataManager.RemoveConference(_newConferenceId);
                _newConferenceId = Guid.Empty;
            }

        }
    }
}
