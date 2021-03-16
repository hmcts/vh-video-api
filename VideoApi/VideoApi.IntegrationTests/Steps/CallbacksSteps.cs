using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using AcceptanceTests.Common.Api.Helpers;
using FizzWare.NBuilder;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TechTalk.SpecFlow;
using Testing.Common.Helper;
using VideoApi.Common;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Requests;
using VideoApi.DAL;
using VideoApi.Domain;
using VideoApi.Extensions;
using static Testing.Common.Helper.ApiUriFactory;
using Task = System.Threading.Tasks.Task;
using TestContext = VideoApi.IntegrationTests.Contexts.TestContext;

namespace VideoApi.IntegrationTests.Steps
{
    [Binding]
    public class CallbackSteps : BaseSteps
    {
        private readonly TestContext _context;
        public CallbackSteps(TestContext context)
        {
            _context = context;
        }

        [Given(@"I have a valid conference event request for event type (.*)")]
        public void GivenIHaveAnConferenceEventRequestForAnEventType(EventType eventType)
        {
            var conference = _context.Test.Conference;
            var request = BuildRequest(eventType, conference);
            _context.Uri = EventsEndpoints.Event;
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = RequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }
        
        [Given(@"I have a transfer judge into consultation room event")]
        public void GivenIHaveTransferToAJudgeIntoConsultationRoomEvent()
        {
            var conference = _context.Test.Conference;
            var room = _context.Test.Room;
            var request = BuildRequest(EventType.Transfer, _context.Test.Conference);
            request.ParticipantId = conference.Participants.First(x => x.IsJudge()).Id.ToString();
            request.TransferFrom = RoomType.WaitingRoom.ToString();
            request.TransferTo = room.Label;
            _context.Uri = EventsEndpoints.Event;
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = RequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }
        
        [Given(@"the judge is in the consultation room")]
        public async Task GivenTheJudgeIsInTheConsultationRoom()
        {
            var conference = _context.Test.Conference;
            var room = _context.Test.Room;
            await using var db = new VideoApiDbContext(_context.VideoBookingsDbContextOptions);
            var judge = conference.Participants.Single(x => x.IsJudge());

            var dbRoom = await db.Rooms.Include(x => x.RoomParticipants).SingleAsync(x=> x.Label == room.Label);
            dbRoom.AddParticipant(new RoomParticipant(judge.Id));
            var record = await db.SaveChangesAsync();
            record.Should().BePositive();
        }
        
        [Given(@"I have a transfer judge out of a consultation room event")]
        public void GivenIHaveTransferJudgeOutOfAConsultationRoomEvent()
        {
            var conference = _context.Test.Conference;
            var room = _context.Test.Room;
            var request = BuildRequest(EventType.Transfer, _context.Test.Conference);
            request.ParticipantId = conference.Participants.First(x => x.IsJudge()).Id.ToString();
            request.TransferFrom = room.Label;
            request.TransferTo = RoomType.WaitingRoom.ToString();
            _context.Uri = EventsEndpoints.Event;
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = RequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }
        
        [Given(@"I have a valid conference phone event request for event type (.*)")]
        public void GivenIHaveAnConferencePhoneEventRequestForAnEventType(EventType eventType)
        {
            var request = BuildRequest(eventType, _context.Test.Conference, "0123456789");
            _context.Uri = EventsEndpoints.Event;
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = RequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have a valid conference event request with a room id for event type (.*)")]
        public void GivenIHaveAValidConferenceEventRequestWithARoomIdForEventType(EventType eventType)
        {
            var roomId = _context.Test.Room.Id;
            var request = BuildRequest(eventType, _context.Test.Conference, null, roomId.ToString());
            _context.Uri = EventsEndpoints.Event;
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = RequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }
        
        [Given(@"I have a valid conference event request with a room id and participant id for event type (.*)")]
        public void GivenIHaveAValidConferenceEventRequestWithARoomIdAndParticipantIdForEventType(EventType eventType)
        {
            var room = _context.Test.Room;
            var participantId = room.RoomParticipants.First().ParticipantId;
            var request = BuildRequest(eventType, _context.Test.Conference, null, room.Id.ToString());
            request.ParticipantId = participantId.ToString();
            _context.Uri = EventsEndpoints.Event;
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = RequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }
        
        [Then(@"the room count should differ by (.*)")]
        public async Task ThenTheRoomCountShouldDifferBy(int countDifference)
        {
            var roomId = _context.Test.Room.Id;
            await using var db = new VideoApiDbContext(_context.VideoBookingsDbContextOptions);
            var updatedConference = db.Conferences
                .Include(x=> x.Rooms).ThenInclude(x=> x.RoomEndpoints)
                .Include(x=> x.Rooms).ThenInclude(x=> x.RoomParticipants)
                .Single(x => x.Id == _context.Test.Conference.Id);

            var updatedRoom = updatedConference.Rooms.First(x => x.Id == roomId);
            var before = _context.Test.Room.RoomParticipants.Count;
            var after = updatedRoom.RoomParticipants.Count;
            (after - before).Should().Be(countDifference);
        }
        
        [Given(@"I have a (.*) conference event request")]
        [Given(@"I have an (.*) conference event request")]
        public void GivenIHaveAnConferenceEventRequest(Scenario scenario)
        {
            var request = scenario switch
            {
                Scenario.Nonexistent => BuildRequest(EventType.Transfer),
                Scenario.Invalid => BuildInvalidRequest(),
                _ => throw new ArgumentOutOfRangeException(nameof(scenario), scenario, null)
            };

            _context.Uri = EventsEndpoints.Event;
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = RequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Given(@"I have a room transfer event request for a nonexistent participant")]
        public void GivenIRoomTransferEventRequestForNonExistentParticipant()
        {
            var request = BuildRequest(EventType.Transfer, _context.Test.Conference);
            request.ParticipantId = Guid.NewGuid().ToString();

            _context.Uri = EventsEndpoints.Event;
            _context.HttpMethod = HttpMethod.Post;
            var jsonBody = RequestHelper.Serialise(request);
            _context.HttpContent = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        [Then(@"the judge should be in the consultation room")]
        public async Task ThenTheParticipantShouldBeInTheConsultationRoom()
        {
            await AssertParticipantStateAndRoom(ParticipantState.InConsultation, true);
        }
        
        [Then(@"the judge should be in the waiting room")]
        public async Task ThenTheParticipantShouldBeInTheWaitingRoom()
        {
            await AssertParticipantStateAndRoom(ParticipantState.Available, false);
        }

        private async Task AssertParticipantStateAndRoom(ParticipantState expectedState, bool shouldBeInRoom)
        {
            var conference = _context.Test.Conference;
            var room = _context.Test.Room;
            await using var db = new VideoApiDbContext(_context.VideoBookingsDbContextOptions);
            var updatedConference = await db.Conferences.Include(x => x.Participants).ThenInclude(x => x.CurrentConsultationRoom)
                .ThenInclude(x => x.RoomParticipants).SingleAsync(x => x.Id == conference.Id);
            var judge = updatedConference.Participants.First(x => x.IsJudge());

            judge.State.Should().Be(expectedState);
            
            var updatedRoom = await db.Rooms.SingleAsync(x => x.Label == room.Label);
            updatedRoom.RoomParticipants.Any(x => x.ParticipantId == judge.Id).Should().Be(shouldBeInRoom);
        }
        
        private ConferenceEventRequest BuildRequest(EventType eventType, Conference conference = null, string phone = null, string participantRoomId = null)
        {
            var request = Builder<ConferenceEventRequest>.CreateNew()
                .With(x => x.ConferenceId = Guid.NewGuid().ToString())
                .With(x => x.ParticipantId = Guid.NewGuid().ToString())
                .With(x => x.EventId = Guid.NewGuid().ToString())
                .With(x => x.EventType = eventType)
                .With(x => x.TransferFrom = RoomType.WaitingRoom.ToString())
                .With(x => x.TransferTo = RoomType.ConsultationRoom.ToString())
                .With(x => x.Reason = "Automated")
                .With(x => x.Phone = phone)
                .With(x => x.ParticipantRoomId = participantRoomId)
                .Build();

            if (conference == null) return request;

            request.ConferenceId = conference.Id.ToString();
            var isEndpointEvent = eventType.MapToDomainEnum().IsEndpointEvent();
            var participantId = isEndpointEvent ? conference.GetEndpoints().First().Id: conference.GetParticipants().First().Id;
            request.ParticipantId = participantId.ToString();
            _context.Test.ParticipantId = participantId;
            return request;
        }

        private static ConferenceEventRequest BuildInvalidRequest()
        {
            var request = Builder<ConferenceEventRequest>.CreateNew()
                .With(x => x.ConferenceId = string.Empty)
                .With(x => x.ParticipantId = string.Empty)
                .With(x => x.EventId = string.Empty)
                .With(x => x.EventType = EventType.None)
                .With(x => x.Reason = "Automated")
                .With(x => x.Phone = null)
                .Build();
            return request;
        }
    }
}
