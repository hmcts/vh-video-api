using System;
using System.Collections.Generic;
using System.Linq;
using Autofac.Extras.Moq;
using Moq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Enums;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Services;
using VideoApi.Services.Clients;
using VideoApi.Services.Contracts;
using Task = System.Threading.Tasks.Task;
using UserRole = VideoApi.Domain.Enums.UserRole;
using VirtualCourtRoomType = VideoApi.Domain.Enums.VirtualCourtRoomType;

namespace VideoApi.UnitTests.Services.VirtualRoom
{
    public class GetOrCreateAJudicialVirtualRoom
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
            var supplerPlatformServiceFactory = _mocker.Mock<ISupplierPlatformServiceFactory>();
            supplerPlatformServiceFactory.Setup(x => x.Create(Supplier.Kinly)).Returns(supplierPlatformService.Object);
            _service = _mocker.Create<VirtualRoomService>();
            _conference = new ConferenceBuilder().WithParticipants(3)
                .WithParticipant(UserRole.JudicialOfficeHolder, "Judicial")
                .WithParticipant(UserRole.JudicialOfficeHolder, "Judicial")
                .Build();
        }

        [Test]
        public async Task should_return_existing_judicial_officer_holder_room()
        {
            // arrange
            var existingInterpreterRoom = new ParticipantRoom(_conference.Id, "Interpreter2", VirtualCourtRoomType.Civilian);
            existingInterpreterRoom.SetProtectedProperty(nameof(existingInterpreterRoom.Id), 2);
            var existingJohRoom = new ParticipantRoom(_conference.Id, "PanelMember1", VirtualCourtRoomType.JudicialShared);
            existingJohRoom.SetProtectedProperty(nameof(existingJohRoom.Id), 1);
            var joh = (Participant)_conference.Participants.First(x => x.UserRole == UserRole.JudicialOfficeHolder);
            
            _mocker.Mock<IQueryHandler>().Setup(x =>
                    x.Handle<GetParticipantRoomsForConferenceQuery, List<ParticipantRoom>>(
                        It.Is<GetParticipantRoomsForConferenceQuery>(q =>
                            q.ConferenceId == _conference.Id)))
                .ReturnsAsync(new List<ParticipantRoom> {existingInterpreterRoom, existingJohRoom});
            
            // act
            var room = await _service.GetOrCreateAJudicialVirtualRoom(_conference, joh);
            
            // assert
            room.Should().Be(existingJohRoom);

        }
        
        [Test]
        public async Task should_create_a_judicial_officer_holder_room_if_one_does_not_exist()
        {
            // arrange
            var expectedRoomId = 2937;
            
            var existingInterpreterRoom = new ParticipantRoom(_conference.Id, "Interpreter2", VirtualCourtRoomType.Civilian);
            existingInterpreterRoom.SetProtectedProperty(nameof(existingInterpreterRoom.Id), 9999);
            var expectedJohRoom = new ParticipantRoom(_conference.Id, "PanelMember1", VirtualCourtRoomType.JudicialShared);
            expectedJohRoom.SetProtectedProperty(nameof(expectedJohRoom.Id), expectedRoomId);
            var newVmrRoom = new BookedParticipantRoomResponse
            {
                Room_label = "PanelMember1",
                Uris = new Uris
                {
                    Participant = "wertyui__panelmember",
                    Pexip_node = "test.node.com"
                }
            };
            
            var joh = (Participant)_conference.Participants.First(x => x.UserRole == UserRole.JudicialOfficeHolder);
            
            _mocker.Mock<IQueryHandler>().SetupSequence(x =>
                    x.Handle<GetParticipantRoomsForConferenceQuery, List<ParticipantRoom>>(
                        It.Is<GetParticipantRoomsForConferenceQuery>(q =>
                            q.ConferenceId == _conference.Id)))
                .ReturnsAsync(new List<ParticipantRoom> {existingInterpreterRoom})
                .ReturnsAsync(new List<ParticipantRoom> {existingInterpreterRoom, expectedJohRoom});
            
            _mocker.Mock<ICommandHandler>().Setup(x =>
                x.Handle(It.IsAny<CreateParticipantRoomCommand>())).Callback<CreateParticipantRoomCommand>(command =>
            {
                command.SetProtectedProperty(nameof(command.NewRoomId), expectedRoomId);
            });
            
            _mocker.Mock<ICommandHandler>().Setup(x =>
                x.Handle(It.IsAny<UpdateParticipantRoomConnectionDetailsCommand>())).Callback(() =>
                expectedJohRoom.UpdateConnectionDetails(newVmrRoom.Room_label, null, newVmrRoom.Uris.Pexip_node,
                    newVmrRoom.Uris.Participant));
            
            _mocker.Mock<ISupplierApiClient>().Setup(x => x.CreateParticipantRoomAsync(_conference.Id.ToString(),
                    It.Is<CreateParticipantRoomParams>(vmrRequest => vmrRequest.Room_type == SupplierRoomType.Panel_Member)))
                .ReturnsAsync(newVmrRoom);
            
            // act
            var room = await _service.GetOrCreateAJudicialVirtualRoom(_conference, joh);
            
            // assert
            room.Should().NotBeNull();
            room.Label.Should().Be(newVmrRoom.Room_label);
            room.PexipNode.Should().Be(newVmrRoom.Uris.Pexip_node);
            room.ParticipantUri.Should().Be(newVmrRoom.Uris.Participant);
            room.IngestUrl.Should().BeNull();
            _mocker.Mock<ISupplierApiClient>().Verify(x=> x.CreateParticipantRoomAsync(_conference.Id.ToString(), 
                It.Is<CreateParticipantRoomParams>(createParams => 
                    createParams.Room_label_prefix == "Panel Member" && 
                    createParams.Participant_type == "Civilian" && 
                    createParams.Room_type == SupplierRoomType.Panel_Member && 
                    createParams.Participant_room_id == expectedRoomId.ToString() &&
                    createParams.Audio_recording_url == string.Empty
                )), Times.Once);
        }
    }
}
