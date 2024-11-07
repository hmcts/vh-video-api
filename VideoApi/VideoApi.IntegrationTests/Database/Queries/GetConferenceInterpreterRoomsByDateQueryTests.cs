using NUnit.Framework;
using System;
using System.Collections.Generic;
using VideoApi.Domain.Enums;
using VideoApi.DAL;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using System.Linq;

namespace VideoApi.IntegrationTests.Database.Queries
{
    public class GetConferenceInterpreterRoomsByDateQueryTests :DatabaseTestsBase
    {
        private GetConferenceInterpreterRoomsByDateQueryHandler _handler;
        private Guid _newConferenceId;
        private List<Room> _rooms;
        private List<Event> _events;

        [SetUp]
        public void Setup()
        {
            _newConferenceId = Guid.Empty;
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetConferenceInterpreterRoomsByDateQueryHandler(context);
        }

        [Test]
        public async System.Threading.Tasks.Task Should_return_an_empty_list_if_conference_does_not_exist()
        {
            var query = new GetConferenceInterpreterRoomsByDateQuery(DateTime.Today);
            var result = await _handler.Handle(query);

            result.Should().BeEmpty();
        }

        [Test]
        public async System.Threading.Tasks.Task Should_return_list_if_conference_status_exist()
        {
            var conference = await TestDataManager.SeedConferenceWithLinkedParticipant();
            _newConferenceId = conference.Id;

            _rooms = CreateRoomsForConference(conference);
            await TestDataManager.SeedRooms(_rooms);

            var interpreter = GetInterpreter(conference.Participants);
            
            _events = CreateEventsForConference(conference.Id, interpreter);
            await TestDataManager.SeedEvents(_events);

            var query = new GetConferenceInterpreterRoomsByDateQuery(DateTime.Today);
            var result = await _handler.Handle(query);

            result.Should().NotBeEmpty();
        }

        private static Guid GetInterpreter(IList<ParticipantBase> participants)
        {
            return participants.Single(x => x.HearingRole == "Interpreter").Id;
        }

        private static List<Event> CreateEventsForConference(Guid conferenceId, Guid participantId)
        {
            var @event = new Event(conferenceId, conferenceId.ToString(), EventType.RoomParticipantTransfer, DateTime.UtcNow, RoomType.WaitingRoom, RoomType.HearingRoom, "", "")
                {
                    ParticipantId = participantId
                };

            return [@event];
        }

        [TearDown]
        public async System.Threading.Tasks.Task TearDown()
        {
            if (_newConferenceId != Guid.Empty)
            {
                TestContext.WriteLine("Cleaning Rooms for GetInterpreterRoomsForConferenceQuery");
                await TestDataManager.RemoveRooms(_newConferenceId);

                TestContext.WriteLine("Cleaning Events for GetInterpreterRoomsForConferenceQuery");
                await TestDataManager.RemoveEvents(_newConferenceId, EventType.RoomParticipantTransfer);

                TestContext.WriteLine("Cleaning conferences for GetInterpreterRoomsForConferenceQuery");
                await TestDataManager.RemoveConference(_newConferenceId);
            }
        }

        private static List<Room> CreateRoomsForConference(Conference conference)
        {
            var room = new ParticipantRoom(conference.Id, "InterpreterRoom1", VirtualCourtRoomType.Civilian);
            
            return [room];
        }

        
    }
}
