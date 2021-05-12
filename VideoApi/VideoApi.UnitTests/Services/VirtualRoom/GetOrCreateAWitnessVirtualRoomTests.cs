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
    public class GetOrCreateAWitnessVirtualRoomTests
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

            var emptyCivilianInterpreterRoom = new ParticipantRoom(_conference.Id, "Interpreter1", VirtualCourtRoomType.Civilian);
            emptyCivilianInterpreterRoom.SetProtectedProperty(nameof(emptyCivilianInterpreterRoom.Id), 1);
            _mocker.Mock<IQueryHandler>().Setup(x =>
                    x.Handle<GetParticipantRoomsForConferenceQuery, List<ParticipantRoom>>(It.Is<GetParticipantRoomsForConferenceQuery>(q =>
                        q.ConferenceId == _conference.Id)))
                .ReturnsAsync(new List<ParticipantRoom> {emptyCivilianInterpreterRoom});
        }

        [Test]
        public async Task should_create_vmr_with_kinly_if_room_is_not_available()
        {
            var expectedRoomId = 2;
            var participant = _conference.Participants.First(x => !x.IsJudge());
            var expectedRoom = new ParticipantRoom(_conference.Id, VirtualCourtRoomType.Witness);
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
                .ReturnsAsync(new List<ParticipantRoom> {expectedRoom});

            _mocker.Mock<ICommandHandler>().Setup(x =>
                x.Handle(It.IsAny<CreateParticipantRoomCommand>())).Callback<CreateParticipantRoomCommand>(command =>
            {
                command.SetProtectedProperty(nameof(command.NewRoomId), expectedRoomId);
            });

            _mocker.Mock<ICommandHandler>().Setup(x =>
                x.Handle(It.IsAny<UpdateParticipantRoomConnectionDetailsCommand>())).Callback(() =>
                expectedRoom.UpdateConnectionDetails(newVmrRoom.Room_label, expectedIngestUrl,
                    newVmrRoom.Uris.Pexip_node,
                    newVmrRoom.Uris.Participant));

            _mocker.Mock<IKinlyApiClient>().Setup(x => x.CreateParticipantRoomAsync(_conference.Id.ToString(),
                    It.Is<CreateParticipantRoomParams>(vmrRequest => vmrRequest.Participant_type == "Witness")))
                .ReturnsAsync(newVmrRoom);
            var room = await _service.GetOrCreateAWitnessVirtualRoom(_conference, participant);
            room.Should().NotBeNull();
            room.Label.Should().Be(newVmrRoom.Room_label);
            room.PexipNode.Should().Be(newVmrRoom.Uris.Pexip_node);
            room.ParticipantUri.Should().Be(newVmrRoom.Uris.Participant);
            room.IngestUrl.Equals(expectedIngestUrl).Should().Be(true);

            _mocker.Mock<IKinlyApiClient>().Verify(x => x.CreateParticipantRoomAsync(_conference.Id.ToString(),
                It.Is<CreateParticipantRoomParams>(createParams =>
                    createParams.Room_label_prefix == "Interpreter" &&
                    createParams.Participant_type == "Witness" &&
                    createParams.Participant_room_id == expectedRoomId.ToString()
                )), Times.Once);
        }

        private Conference InitConference()
        {
            var conference = new ConferenceBuilder().WithParticipants(3).Build();
            var participantA = conference.Participants[0];
            var participantB = conference.Participants[1];

            participantA.AddLink(participantB.Id, LinkedParticipantType.Interpreter);
            participantA.SetProtectedProperty(nameof(participantA.HearingRole), "Witness");
            participantB.AddLink(participantA.Id, LinkedParticipantType.Interpreter);

            return conference;
        }
    }
}
