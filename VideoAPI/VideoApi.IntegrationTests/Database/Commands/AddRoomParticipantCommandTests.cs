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
    public class AddRoomParticipantCommandTests : DatabaseTestsBase
    {
        private AddRoomParticipantCommandHandler _handler;
        private Guid _newConferenceId;
        private long _newRoomId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new AddRoomParticipantCommandHandler(context);
        }


        [Test]
        public async Task Should_add_participant_to_room()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var judge = seededConference.Participants.FirstOrDefault(x => x.UserRole == UserRole.Judge);
            var room = await TestDataManager.SeedRoom(new Room(_newConferenceId, "Room1", VirtualCourtRoomType.JudgeJOH));
            _newRoomId = room.Id;
            var enterTime = DateTime.UtcNow;
            var command = new AddRoomParticipantCommand(_newRoomId, new RoomParticipant(_newRoomId, judge.Id, enterTime));

            await _handler.Handle(command);
            Room roomSaved;
            await using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            {
                roomSaved = await db.Rooms.Include("RoomParticipants")
                    .SingleOrDefaultAsync(x => x.Id == _newRoomId);
            }

            var savedRoomParticipant = roomSaved.RoomParticipants.First();


            savedRoomParticipant.Should().NotBeNull();
            savedRoomParticipant.RoomId.Should().Be(_newRoomId);
            savedRoomParticipant.ParticipantId.Should().Be(judge.Id);
            savedRoomParticipant.EnterTime.Should().Be(enterTime);

        }

        [Test]
        public void Should_throw_exception_if_no_room_found()
        {
            var enterTime = DateTime.UtcNow;
            var command = new AddRoomParticipantCommand(_newRoomId, new RoomParticipant(_newRoomId, Guid.NewGuid(), enterTime));
            Assert.ThrowsAsync<RoomNotFoundException>(() => _handler.Handle(command));
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
            }

        }
    }
}
