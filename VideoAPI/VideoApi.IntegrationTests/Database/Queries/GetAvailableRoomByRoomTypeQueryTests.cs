using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Queries
{
    public class GetAvailableRoomByRoomTypeQueryTests : DatabaseTestsBase
    {
        private GetAvailableRoomByRoomTypeQueryHandler _handler;
        private UpdateRoomStatusCommandHandler _handlerUpdate;
        private Guid _newConferenceId;
        private List<long> _expectedIds;
        private List<long> _notExpectedIds;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetAvailableRoomByRoomTypeQueryHandler(context);
            _handlerUpdate = new UpdateRoomStatusCommandHandler(context);

        }

        [Test]
        public async Task Should_returns_room_with_status_live_or_created_for_the_given_room_type()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var listRooms = GetListRoom(_newConferenceId);
            var roomSaved = await TestDataManager.SeedRooms(listRooms);
            _expectedIds = new List<long> { roomSaved[1].Id, roomSaved[2].Id, roomSaved[3].Id };
            _notExpectedIds = new List<long> { roomSaved[0].Id, roomSaved[4].Id };

            var command = new UpdateRoomStatusCommand(roomSaved[0].Id, RoomStatus.Closed);
            await _handlerUpdate.Handle(command);

            var query = new GetAvailableRoomByRoomTypeQuery(VirtualCourtRoomType.JudgeJOH);
            var result = await _handler.Handle(query);

            result.Should().NotBeEmpty();
            result.Any(x => x.Id == _expectedIds[0]).Should().Be(true);
            result.Any(x => x.Id == _expectedIds[1]).Should().Be(true);
            result.Any(x => x.Id == _expectedIds[2]).Should().Be(true);
            result.Any(x => x.Id == _notExpectedIds[0]).Should().Be(false);
            result.Any(x => x.Id == _notExpectedIds[1]).Should().Be(false);

        }

        private List<Room> GetListRoom(Guid conferenceId)
        {
            return new List<Room>
            {
                new Room(conferenceId, "RoomTest", VirtualCourtRoomType.JudgeJOH),
                new Room(conferenceId, "RoomTest", VirtualCourtRoomType.JudgeJOH),
                new Room(conferenceId, "RoomTest", VirtualCourtRoomType.JudgeJOH),
                new Room(conferenceId, "RoomTest", VirtualCourtRoomType.JudgeJOH),
                new Room(conferenceId, "RoomTest", VirtualCourtRoomType.Participant),
            };
        }

        [TearDown]
        public async Task TearDown()
        {
            TestContext.WriteLine("Cleaning rooms for GetAvailableRoomByRoomTypeQuery");
            await TestDataManager.RemoveRooms(_newConferenceId);

            TestContext.WriteLine("Cleaning conferences for GetAvailableRoomByRoomTypeQuery");
            await TestDataManager.RemoveConference(_newConferenceId);
        }
    }
}
