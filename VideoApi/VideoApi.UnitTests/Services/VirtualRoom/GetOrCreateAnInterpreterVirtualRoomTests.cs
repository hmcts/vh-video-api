using Autofac.Extras.Moq;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Services;
using VideoApi.Services.Clients;
using VideoApi.Services.Contracts;
using LinkedParticipantType = VideoApi.Domain.Enums.LinkedParticipantType;
using Task = System.Threading.Tasks.Task;
using VirtualCourtRoomType = VideoApi.Domain.Enums.VirtualCourtRoomType;

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
            var supplierPlatformService = _mocker.Mock<IVideoPlatformService>();
            supplierPlatformService.Setup(x => x.GetHttpClient()).Returns(_mocker.Mock<ISupplierApiClient>().Object);
            var supplierPlatformServiceFactory = _mocker.Mock<ISupplierPlatformServiceFactory>();
            supplierPlatformServiceFactory.Setup(x => x.Create(VideoApi.Domain.Enums.Supplier.Kinly)).Returns(supplierPlatformService.Object);
            supplierPlatformServiceFactory.Setup(x => x.Create(VideoApi.Domain.Enums.Supplier.Vodafone)).Returns(supplierPlatformService.Object);
            _service = _mocker.Create<VirtualRoomService>();
            _conference = InitConference();
            
            var emptyWitnessInterpreterRoom = new ParticipantRoom(_conference.Id, "Interpreter1", VirtualCourtRoomType.Witness);
            emptyWitnessInterpreterRoom.SetProtectedProperty(nameof(emptyWitnessInterpreterRoom.Id), 1);
            _mocker.Mock<IQueryHandler>().Setup(x =>
                    x.Handle<GetParticipantRoomsForConferenceQuery, List<ParticipantRoom>>(It.Is<GetParticipantRoomsForConferenceQuery>(q =>
                        q.ConferenceId == _conference.Id)))
                .ReturnsAsync(new List<ParticipantRoom> {emptyWitnessInterpreterRoom});
        }

        [Test]
        public async Task should_reuse_empty_interpreter_room()
        {
            var participant = _conference.Participants[0];
            var emptyInterpreterRoom = new ParticipantRoom(_conference.Id, "Interpreter2", VirtualCourtRoomType.Civilian);
            emptyInterpreterRoom.SetProtectedProperty(nameof(emptyInterpreterRoom.Id), 2);

            _mocker.Mock<IQueryHandler>().Setup(x =>
                    x.Handle<GetParticipantRoomsForConferenceQuery, List<ParticipantRoom>>(It.Is<GetParticipantRoomsForConferenceQuery>(q =>
                        q.ConferenceId == _conference.Id)))
                .ReturnsAsync(new List<ParticipantRoom> {emptyInterpreterRoom});

            var room = await _service.GetOrCreateAnInterpreterVirtualRoom(_conference, participant);
            room.Should().NotBeNull();
            room.Should().BeEquivalentTo(emptyInterpreterRoom);
        }
        
        [Test]
        public async Task should_return_room_with_linked_participant()
        {
            var participant = _conference.Participants[0];
            var participantB = _conference.Participants[1];
            var emptyInterpreterRoom = new ParticipantRoom(_conference.Id, "Interpreter2", VirtualCourtRoomType.Civilian);
            emptyInterpreterRoom.SetProtectedProperty(nameof(emptyInterpreterRoom.Id), 2);
            var interpreterRoom = new ParticipantRoom(_conference.Id, "Interpreter3", VirtualCourtRoomType.Civilian);
            interpreterRoom.SetProtectedProperty(nameof(emptyInterpreterRoom.Id), 3);
            interpreterRoom.AddParticipant(new RoomParticipant(participantB.Id));
            

            _mocker.Mock<IQueryHandler>().Setup(x =>
                    x.Handle<GetParticipantRoomsForConferenceQuery, List<ParticipantRoom>>(It.Is<GetParticipantRoomsForConferenceQuery>(q =>
                        q.ConferenceId == _conference.Id)))
                .ReturnsAsync(new List<ParticipantRoom> {emptyInterpreterRoom, interpreterRoom});

            var room = await _service.GetOrCreateAnInterpreterVirtualRoom(_conference, participant);
            room.Should().NotBeNull();
            room.Should().BeEquivalentTo(interpreterRoom);
        }

        [Test]
        public async Task should_create_vmr_with_kinly_if_room_is_not_available()
        {
            var expectedRoomId = 2;
            var participant = _conference.Participants.First(x => x is Participant && !((Participant)x).IsJudge());
            var expectedRoom = new ParticipantRoom(_conference.Id, VirtualCourtRoomType.Civilian);
            var interpreterSuffix = "_interpreter_";
            var expectedIngestUrl = $"{_conference.IngestUrl}{interpreterSuffix}{expectedRoomId}";

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
                    x.Handle<GetParticipantRoomsForConferenceQuery, List<ParticipantRoom>>(It.Is<GetParticipantRoomsForConferenceQuery>(q =>
                        q.ConferenceId == _conference.Id)))
                .ReturnsAsync(new List<ParticipantRoom>())
                .ReturnsAsync(new List<ParticipantRoom>{expectedRoom});

            _mocker.Mock<ICommandHandler>().Setup(x =>
                x.Handle(It.IsAny<CreateParticipantRoomCommand>())).Callback<CreateParticipantRoomCommand>(command =>
            {
                command.SetProtectedProperty(nameof(command.NewRoomId), expectedRoomId);
            });
            
            _mocker.Mock<ICommandHandler>().Setup(x =>
                x.Handle(It.IsAny<UpdateParticipantRoomConnectionDetailsCommand>())).Callback(() =>
                expectedRoom.UpdateConnectionDetails(newVmrRoom.Room_label, expectedIngestUrl, newVmrRoom.Uris.Pexip_node,
                    newVmrRoom.Uris.Participant));

            _mocker.Mock<ISupplierApiClient>().Setup(x => x.CreateParticipantRoomAsync(_conference.Id.ToString(),
                    It.Is<CreateParticipantRoomParams>(vmrRequest => vmrRequest.Participant_type == "Civilian")))
                .ReturnsAsync(newVmrRoom);
            var room = await _service.GetOrCreateAnInterpreterVirtualRoom(_conference, participant);

            room.Should().NotBeNull();
            room.Label.Should().Be(newVmrRoom.Room_label);
            room.PexipNode.Should().Be(newVmrRoom.Uris.Pexip_node);
            room.ParticipantUri.Should().Be(newVmrRoom.Uris.Participant);
            room.IngestUrl.Should().Be(expectedIngestUrl);

            _mocker.Mock<ISupplierApiClient>().Verify(x=> x.CreateParticipantRoomAsync(_conference.Id.ToString(), 
                It.Is<CreateParticipantRoomParams>(createParams => 
                    createParams.Room_label_prefix == "Interpreter" && 
                    createParams.Participant_type == "Civilian" && 
                    createParams.Participant_room_id == expectedRoomId.ToString()
                    
                )), Times.Once);
        }

        private static Conference InitConference()
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
