using System.Collections.Generic;
using System.Linq;
using Autofac.Extras.Moq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services;
using VideoApi.Services.Kinly;
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
            
            var emptyWitnessInterpreterRoom = new InterpreterRoom(_conference.Id, "Interpreter1", VirtualCourtRoomType.Witness);
            emptyWitnessInterpreterRoom.SetProtectedProperty(nameof(emptyWitnessInterpreterRoom.Id), 1);
            _mocker.Mock<IQueryHandler>().Setup(x =>
                    x.Handle<GetInterpreterRoomsForConferenceQuery, List<InterpreterRoom>>(It.Is<GetInterpreterRoomsForConferenceQuery>(q =>
                        q.ConferenceId == _conference.Id)))
                .ReturnsAsync(new List<InterpreterRoom> {emptyWitnessInterpreterRoom});
        }

        [Test]
        public async Task should_reuse_empty_interpreter_room()
        {
            var participant = _conference.Participants[0];
            var emptyInterpreterRoom = new InterpreterRoom(_conference.Id, "Interpreter2", VirtualCourtRoomType.Civilian);
            emptyInterpreterRoom.SetProtectedProperty(nameof(emptyInterpreterRoom.Id), 2);

            _mocker.Mock<IQueryHandler>().Setup(x =>
                    x.Handle<GetInterpreterRoomsForConferenceQuery, List<InterpreterRoom>>(It.Is<GetInterpreterRoomsForConferenceQuery>(q =>
                        q.ConferenceId == _conference.Id)))
                .ReturnsAsync(new List<InterpreterRoom> {emptyInterpreterRoom});

            var room = await _service.GetOrCreateAnInterpreterVirtualRoom(_conference, participant);
            room.Should().NotBeNull();
            room.Should().BeEquivalentTo(emptyInterpreterRoom);
        }
        
        [Test]
        public async Task should_return_room_with_linked_participant()
        {
            var participant = _conference.Participants[0];
            var participantB = _conference.Participants[1];
            var emptyInterpreterRoom = new InterpreterRoom(_conference.Id, "Interpreter2", VirtualCourtRoomType.Civilian);
            emptyInterpreterRoom.SetProtectedProperty(nameof(emptyInterpreterRoom.Id), 2);
            var interpreterRoom = new InterpreterRoom(_conference.Id, "Interpreter3", VirtualCourtRoomType.Civilian);
            interpreterRoom.SetProtectedProperty(nameof(emptyInterpreterRoom.Id), 3);
            interpreterRoom.AddParticipant(new RoomParticipant(participantB.Id));
            

            _mocker.Mock<IQueryHandler>().Setup(x =>
                    x.Handle<GetInterpreterRoomsForConferenceQuery, List<InterpreterRoom>>(It.Is<GetInterpreterRoomsForConferenceQuery>(q =>
                        q.ConferenceId == _conference.Id)))
                .ReturnsAsync(new List<InterpreterRoom> {emptyInterpreterRoom, interpreterRoom});

            var room = await _service.GetOrCreateAnInterpreterVirtualRoom(_conference, participant);
            room.Should().NotBeNull();
            room.Should().BeEquivalentTo(interpreterRoom);
        }

        [Test]
        public async Task should_create_vmr_with_kinly_if_room_is_not_available()
        {
            var expectedRoomId = 2;
            var participant = _conference.Participants.First(x => !x.IsJudge());
            var expectedRoom = new InterpreterRoom(_conference.Id, VirtualCourtRoomType.Civilian);
            expectedRoom.SetProtectedProperty(nameof(expectedRoom.Id), expectedRoomId);
            var newVmrRoom = new BookedParticipantRoomResponse
            {
                Room_label = "Interpreter2",
                Uris = new Uris
                {
                    Participant = "wertyui__interpreter",
                    Pexip_node = "test.node.com"
                }
            };
            
            _mocker.Mock<IQueryHandler>().SetupSequence(x =>
                    x.Handle<GetInterpreterRoomsForConferenceQuery, List<InterpreterRoom>>(It.Is<GetInterpreterRoomsForConferenceQuery>(q =>
                        q.ConferenceId == _conference.Id)))
                .ReturnsAsync(new List<InterpreterRoom>())
                .ReturnsAsync(new List<InterpreterRoom>{expectedRoom});

            _mocker.Mock<ICommandHandler>().Setup(x =>
                x.Handle(It.IsAny<CreateInterpreterRoomCommand>())).Callback<CreateInterpreterRoomCommand>(command =>
            {
                command.SetProtectedProperty(nameof(command.NewRoomId), expectedRoomId);
            });
            
            _mocker.Mock<ICommandHandler>().Setup(x =>
                x.Handle(It.IsAny<UpdateInterpreterRoomConnectionDetailsCommand>())).Callback(() =>
                expectedRoom.UpdateConnectionDetails(newVmrRoom.Room_label, "ingesturl", newVmrRoom.Uris.Pexip_node,
                    newVmrRoom.Uris.Participant));

            _mocker.Mock<IKinlyApiClient>().Setup(x => x.CreateParticipantRoomAsync(_conference.Id.ToString(),
                    It.Is<CreateParticipantRoomParams>(vmrRequest => vmrRequest.Participant_type == "Civilian")))
                .ReturnsAsync(newVmrRoom);
            
            var room = await _service.GetOrCreateAnInterpreterVirtualRoom(_conference, participant);

            room.Should().NotBeNull();
            room.Label.Should().Be(newVmrRoom.Room_label);
            room.PexipNode.Should().Be(newVmrRoom.Uris.Pexip_node);
            room.ParticipantUri.Should().Be(newVmrRoom.Uris.Participant);
            
            _mocker.Mock<IKinlyApiClient>().Verify(x=> x.CreateParticipantRoomAsync(_conference.Id.ToString(), 
                It.Is<CreateParticipantRoomParams>(createParams => 
                    createParams.Room_label_prefix == "Interpreter" && 
                    createParams.Participant_type == "Civilian" && 
                    createParams.Participant_room_id == expectedRoomId.ToString()
                    
                )), Times.Once);
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
