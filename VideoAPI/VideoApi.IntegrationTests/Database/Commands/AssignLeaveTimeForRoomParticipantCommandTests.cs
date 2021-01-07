using FluentAssertions;
using NUnit.Framework;
using System;
using System.Linq;
using VideoApi.DAL;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Commands
{
    public class AssignLeaveTimeForRoomParticipantCommandTests : DatabaseTestsBase
    {
        private AssignLeaveTimeForRoomParticipantCommandHandler _handler;
        private Guid _newConferenceId;
        private long _newRoomId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new AssignLeaveTimeForRoomParticipantCommandHandler(context);
        }

        [Test]
        public async Task Should_assign_a_leave_time_for_room_participant()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var participant = seededConference.Participants.FirstOrDefault(x => x.UserRole == UserRole.Judge);

            var room = await TestDataManager.SeedRoom(new Room(_newConferenceId, "Room1", VirtualCourtRoomType.JudgeJOH));
            _newRoomId = room.Id;

            var roomWithParticipant = await TestDataManager.SeedRoomWithRoomParticipant(_newRoomId, new RoomParticipant(_newRoomId, participant.Id, DateTime.UtcNow));
            roomWithParticipant.RoomParticipants.Count.Should().Be(1);

            var leaveTime = DateTime.UtcNow;

            var command = new AssignLeaveTimeForRoomParticipantCommand(_newRoomId, participant.Id, leaveTime);

            await _handler.Handle(command);

            var updatedRoom = await TestDataManager.GetRoomById(_newRoomId);

            var updatedRoomParticipant = updatedRoom.RoomParticipants.FirstOrDefault(x => x.ParticipantId == participant.Id);

            updatedRoomParticipant.LeaveTime.Should().Be(leaveTime);
        }

        [Test]
        public async Task Should_throw_exception_if_no_room_found()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var participant = seededConference.Participants.FirstOrDefault(x => x.UserRole == UserRole.Judge);

            var room = await TestDataManager.SeedRoom(new Room(_newConferenceId, "Room1", VirtualCourtRoomType.JudgeJOH));
            _newRoomId = room.Id;

            TestContext.WriteLine($"Removing test room for conference {_newConferenceId}");
            await TestDataManager.RemoveRooms(_newConferenceId);

            var command = new AssignLeaveTimeForRoomParticipantCommand(_newRoomId, participant.Id, DateTime.UtcNow);
            Assert.ThrowsAsync<RoomNotFoundException>(() => _handler.Handle(command));
        }

        [Test]
        public async Task Should_throw_exception_if_no_room_participant_found()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var participant = seededConference.Participants.FirstOrDefault(x => x.UserRole == UserRole.Judge);

            var room = await TestDataManager.SeedRoom(new Room(_newConferenceId, "Room1", VirtualCourtRoomType.JudgeJOH));
            _newRoomId = room.Id;

            var command = new AssignLeaveTimeForRoomParticipantCommand(_newRoomId, participant.Id, DateTime.UtcNow);
            Assert.ThrowsAsync<RoomParticipantNotFoundException>(() => _handler.Handle(command));
        }

        [TearDown]
        public async Task TearDown()
        {
            if(_newRoomId > 0)
            {
                TestContext.WriteLine($"Removing test room for conference {_newConferenceId}");
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
