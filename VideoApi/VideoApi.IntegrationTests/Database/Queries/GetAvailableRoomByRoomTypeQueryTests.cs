using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Queries
{
    public class GetAvailableRoomByRoomTypeQueryTests : DatabaseTestsBase
    {
        private GetAvailableRoomByRoomTypeQueryHandler _handler;
        private Guid _newConferenceId;
        private List<long> _expectedIds;
        private List<long> _notExpectedIds;

        [SetUp]
        public void Setup()
        {
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetAvailableRoomByRoomTypeQueryHandler(context);

        }

        [Test]
        public async Task Should_returns_room_with_status_live_or_created_for_the_given_room_type()
        {
            var seededConference = await TestDataManager.SeedConference();
            TestContext.WriteLine($"New seeded conference id: {seededConference.Id}");
            _newConferenceId = seededConference.Id;
            var participant = seededConference.Participants.First(x => x.UserRole == UserRole.Judge);

            var listRooms = GetListRoom(_newConferenceId);
            var roomSaved = await TestDataManager.SeedRooms(listRooms);
            await TestDataManager.SeedRoomWithRoomParticipant(roomSaved[0].Id, new RoomParticipant(participant.Id));
           

            _expectedIds = new List<long> { roomSaved[1].Id, roomSaved[2].Id, roomSaved[3].Id };
            _notExpectedIds = new List<long> { roomSaved[0].Id, roomSaved[4].Id };

            await using var db = new VideoApiDbContext(VideoBookingsDbContextOptions);
            
            var roomToUpdate = await db.Rooms.Include(x=> x.RoomParticipants).SingleAsync(x => x.Id == roomSaved[0].Id);
            roomToUpdate.RemoveParticipant(new RoomParticipant(participant.Id));
            await db.SaveChangesAsync();

            var query = new GetAvailableRoomByRoomTypeQuery(VirtualCourtRoomType.JudgeJOH, _newConferenceId);
            var result = await _handler.Handle(query);

            result.Should().NotBeEmpty();
            result.Any(x => x.Id == _expectedIds[0]).Should().Be(true);
            result.Any(x => x.Id == _expectedIds[1]).Should().Be(true);
            result.Any(x => x.Id == _expectedIds[2]).Should().Be(true);
            result.Any(x => x.Id == _notExpectedIds[0]).Should().Be(false);
            result.Any(x => x.Id == _notExpectedIds[1]).Should().Be(false);

            await TearDown();
        }

        [Test]
        public void Should_Throw_Conference_Not_Found_Exception_If_Conference_Does_Not_Exist()
        {
            var fakeConferenceId = Guid.NewGuid();
            var query = new GetAvailableRoomByRoomTypeQuery(VirtualCourtRoomType.JudgeJOH, fakeConferenceId);
            _handler.Invoking(x => x.Handle(query)).Should().ThrowAsync<ConferenceNotFoundException>();
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
        
        public async Task TearDown()
        {
            TestContext.WriteLine("Cleaning conferences for GetAvailableRoomByRoomTypeQuery");
            await TestDataManager.RemoveConference(_newConferenceId);
            
            TestContext.WriteLine("Cleaning rooms for GetAvailableRoomByRoomTypeQuery");
            await TestDataManager.RemoveRooms(_newConferenceId);
        }
    }
}