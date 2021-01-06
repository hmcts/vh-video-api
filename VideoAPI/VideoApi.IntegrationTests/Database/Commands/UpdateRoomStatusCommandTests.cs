using FluentAssertions;
using NUnit.Framework;
using System;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class UpdateRoomStatusCommandTests : DatabaseTestsBase
    {
        private UpdateRoomStatusCommandHandler _handler;
        private Guid _newConferenceId;
        private long _newRoomId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new UpdateRoomStatusCommandHandler(context);
        }


        [Test]
        public async Task Should_update_the_room_status()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;

            var room = await TestDataManager.SeedRoom(new Room(_newConferenceId, "Room1", VirtualCourtRoomType.JudgeJOH));
            _newRoomId = room.Id;
            var command = new UpdateRoomStatusCommand(_newRoomId, RoomStatus.Live);

            await _handler.Handle(command);

            command.Status.Should().Be(RoomStatus.Live);

        }

        [Test]
        public async Task Should_throw_exception_if_no_room_found()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;

            var room = await TestDataManager.SeedRoom(new Room(_newConferenceId, "Room1", VirtualCourtRoomType.JudgeJOH));
            _newRoomId = room.Id;

            TestContext.WriteLine($"Removing test room for conference {_newConferenceId}");
            await TestDataManager.RemoveRooms(_newConferenceId);

            var command = new UpdateRoomStatusCommand(_newRoomId, RoomStatus.Live);
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
