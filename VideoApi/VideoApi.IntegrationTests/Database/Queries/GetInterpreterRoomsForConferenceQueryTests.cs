using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using VideoApi.DAL;
using VideoApi.DAL.Queries;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.IntegrationTests.Database.Queries
{
    public class GetInterpreterRoomsForConferenceQueryTests : DatabaseTestsBase
    {
        private GetParticipantRoomsForConferenceQueryHandler _handler;
        private Guid _newConferenceId;
        private List<Room> _rooms;

        [SetUp]
        public void Setup()
        {
            _newConferenceId = Guid.Empty;
            _rooms = null;
            var context = new VideoApiDbContext(VideoBookingsDbContextOptions);
            _handler = new GetParticipantRoomsForConferenceQueryHandler(context);
        }

        [Test]
        public async Task Should_return_an_empty_list_if_conference_does_not_exist()
        {
            var fakeConferenceId = Guid.NewGuid();
            var query = new GetParticipantRoomsForConferenceQuery(fakeConferenceId);
            var result = await _handler.Handle(query);

            result.Should().BeEmpty();
        }

        [Test]
        public async Task should_return_list_of_interpreter_rooms_for_conference()
        {
            var conference = await TestDataManager.SeedConference();
            _newConferenceId = conference.Id;
            _rooms = CreateRoomsForConference(conference.Id);
            await TestDataManager.SeedRooms(_rooms);
            var interpreterRooms = _rooms.Where(r => r is ParticipantRoom).ToList();
            
            var query = new GetParticipantRoomsForConferenceQuery(conference.Id);
            var result = await _handler.Handle(query);


            result.Count.Should().Be(interpreterRooms.Count);
            result.Select(r => r.Id).Should().Contain(interpreterRooms.Select(r => r.Id));
        }
        
        [TearDown]
        public async Task TearDown()
        {
            if (_newConferenceId != Guid.Empty)
            {
                TestContext.WriteLine("Cleaning rooms for GetInterpreterRoomsForConferenceQuery");
                await TestDataManager.RemoveRooms(_newConferenceId);

                TestContext.WriteLine("Cleaning conferences for GetInterpreterRoomsForConferenceQuery");
                await TestDataManager.RemoveConference(_newConferenceId);
            }
        }

        private static List<Room> CreateRoomsForConference(Guid conferenceId)
        {
            return
            [
                new ConsultationRoom(conferenceId, "ConsultationRoom1", VirtualCourtRoomType.Participant, false),
                new ConsultationRoom(conferenceId, "JudgeJOHConsultationRoom1", VirtualCourtRoomType.JudgeJOH, false),
                new ParticipantRoom(conferenceId, "InterpreterRoom1", VirtualCourtRoomType.Witness),
                new ParticipantRoom(conferenceId, VirtualCourtRoomType.Civilian)
            ];
        }
    }
}
