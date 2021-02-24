using System.Collections.Generic;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.UnitTests.Services.VirtualRoom
{
    public class GetOrCreateAnInterpreterVirtualRoomTests
    {
        private AutoMock _mocker;
        private VirtualRoomService _service;
        private Conference _conference;

        [SetUp]
        public void Setup()
        {
            _mocker = AutoMock.GetLoose();
            _service = _mocker.Create<VirtualRoomService>();
            _conference = InitConference();
        }

        [Test]
        public async Task should_reuse_empty_interpreter_room()
        {
            var participant = _conference.Participants[0];
            var emptyInterpreterRoom = new Room(_conference.Id, "InterpreterRoom1", VirtualCourtRoomType.Civilian,
                false);

            _mocker.Mock<IQueryHandler>().Setup(x =>
                    x.Handle<GetAvailableRoomByRoomTypeQuery, List<Room>>(It.Is<GetAvailableRoomByRoomTypeQuery>(q =>
                        q.ConferenceId == _conference.Id && q.CourtRoomType == VirtualCourtRoomType.Civilian)))
                .ReturnsAsync(new List<Room> {emptyInterpreterRoom});

            var room = await _service.GetOrCreateAnInterpreterVirtualRoom(_conference, participant);
            room.Should().NotBeNull();
            room.Should().BeEquivalentTo(emptyInterpreterRoom);
        }
        
        [Test]
        public async Task should_return_room_with_linked_participant()
        {
            var participant = _conference.Participants[0];
            var participantB = _conference.Participants[1];
            var emptyInterpreterRoom = new Room(_conference.Id, "InterpreterRoom1", VirtualCourtRoomType.Civilian,
                false);
            var interpreterRoom = new Room(_conference.Id, "InterpreterRoom2", VirtualCourtRoomType.Civilian,
                false);
            interpreterRoom.AddParticipant(new RoomParticipant(participantB.Id));
            

            _mocker.Mock<IQueryHandler>().Setup(x =>
                    x.Handle<GetAvailableRoomByRoomTypeQuery, List<Room>>(It.Is<GetAvailableRoomByRoomTypeQuery>(q =>
                        q.ConferenceId == _conference.Id && q.CourtRoomType == VirtualCourtRoomType.Civilian)))
                .ReturnsAsync(new List<Room> {emptyInterpreterRoom, interpreterRoom});

            var room = await _service.GetOrCreateAnInterpreterVirtualRoom(_conference, participant);
            room.Should().NotBeNull();
            room.Should().BeEquivalentTo(interpreterRoom);
        }

        private Conference InitConference()
        {
            var conference = new ConferenceBuilder().WithParticipants(3).Build();
            var participantA = conference.Participants[0];
            var participantB = conference.Participants[1];
            
            participantA.AddLink(participantB.Id, LinkedParticipantType.Interpreter);
            participantB.AddLink(participantA.Id, LinkedParticipantType.Interpreter);
            
            return conference;
        }
    }
}
