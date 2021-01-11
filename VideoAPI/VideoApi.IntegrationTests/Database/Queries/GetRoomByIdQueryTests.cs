using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Queries
{
    public class GetRoomByIdQueryTests : DatabaseTestsBase
    {
        private GetRoomByIdQueryHandler _handler;
        private Guid _newConferenceId;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetRoomByIdQueryHandler(context);
            _newConferenceId = Guid.Empty;
        }

        [Test]
        public async Task should_get_room_and_room_participants_for_the_given_room_id()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;

            var participant = seededConference.Participants.First(x => x.UserRole == UserRole.Judge);
            var listRooms = GetListRoom(_newConferenceId);
            var roomSaved = await TestDataManager.SeedRooms(listRooms);
            await TestDataManager.SeedRoomWithRoomParticipant(roomSaved[0].Id, new RoomParticipant(roomSaved[0].Id, participant.Id));

            var room = await _handler.Handle(new GetRoomByIdQuery(roomSaved[0].Id));

            room.Should().NotBeNull();
            room.ConferenceId.Should().Be(seededConference.Id);
            room.RoomParticipants.Count.Should().Be(1);
        }

        private List<Room> GetListRoom(Guid conferenceId)
        {
            return new List<Room>
            {
                new Room(conferenceId, "RoomTest", VirtualCourtRoomType.JudgeJOH),
            };
        }

        [TearDown]
        public async Task TearDown()
        {
            TestContext.WriteLine("Cleaning rooms for GetRoomByIdQuery");
            await TestDataManager.RemoveRooms(_newConferenceId);

            TestContext.WriteLine("Cleaning conferences for GetRoomByIdQuery");
            await TestDataManager.RemoveConference(_newConferenceId);
        }
    }
}
