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
        public async Task Should_remove_participant_from_room()
        {
            //var seededConference = await TestDataManager.SeedConference();
            //TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            //_newConferenceId = seededConference.Id;

            //var judge = seededConference.Participants.FirstOrDefault(x => x.UserRole == UserRole.Judge);
            //var room = await TestDataManager.SeedRoom(new Room(_newConferenceId, "Room1", VirtualCourtRoomType.JudgeJOH));
            //_newRoomId = room.Id;

            //var enterTime = DateTime.UtcNow;
            //var command = new AddRoomParticipantCommand(_newRoomId, new RoomParticipant(_newRoomId, judge.Id, enterTime));

            //await _handler.Handle(command);
            //Room roomSaved;
            //await using (var db = new VideoApiDbContext(VideoBookingsDbContextOptions))
            //{
            //    roomSaved = await db.Rooms.Include("RoomParticipants")
            //        .SingleOrDefaultAsync(x => x.Id == _newRoomId);
            //}

            //var savedRoomParticipant = roomSaved.RoomParticipants.First();


            //savedRoomParticipant.Should().NotBeNull();
            //savedRoomParticipant.RoomId.Should().Be(_newRoomId);
            //savedRoomParticipant.ParticipantId.Should().Be(judge.Id);
            //savedRoomParticipant.EnterTime.Should().Be(enterTime);

        }
    }
}
