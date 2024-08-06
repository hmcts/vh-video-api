using System.Linq;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Consts;
using VideoApi.Domain.Enums;
using VideoApi.Mappings;

namespace VideoApi.UnitTests.Mappings
{
    public class ConferenceToDetailsResponseMapperTests
    {
        [Test]
        public void Should_map_all_properties()
        {
            var conference = new ConferenceBuilder()
                .WithConferenceStatus(ConferenceState.InSession)
                .WithConferenceStatus(ConferenceState.Paused)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithMeetingRoom("https://poc.node.com", "user@hmcts.net")
                .WithParticipants(3)
                .WithMessages(5)
                .WithInterpreterRoom()
                .Build();

            var pexipSelfTestNode = "selttest@pexip.node";
            var response = ConferenceToDetailsResponseMapper.MapConferenceToResponse(conference, pexipSelfTestNode);
            response.Should().BeEquivalentTo(conference, options => options
                .Excluding(x => x.HearingRefId)
                .Excluding(x => x.Participants)
                .Excluding(x => x.ConferenceStatuses)
                .Excluding(x => x.State)
                .Excluding(x => x.InstantMessageHistory)
                .Excluding(x => x.IngestUrl)
                .Excluding(x => x.ActualStartTime)
                .Excluding(x => x.Endpoints)
                .Excluding(x => x.CreatedDateTime)
                .Excluding(x => x.Rooms)
                .Excluding(x => x.UpdatedAt)
                .Excluding(x => x.Supplier)
             );

            response.StartedDateTime.Should().HaveValue().And.Be(conference.ActualStartTime);
            response.ClosedDateTime.Should().HaveValue().And.Be(conference.ClosedDateTime);
            response.CurrentStatus.Should().Be((Contract.Enums.ConferenceState)conference.GetCurrentStatus());

            var participants = conference.GetParticipants();
            response.Participants.Should().BeEquivalentTo(participants, options => options
                .Excluding(x => x.ParticipantRefId)
                .Excluding(x => x.ConferenceId)
                .Excluding(x => x.TestCallResultId)
                .Excluding(x => x.TestCallResult)
                .Excluding(x => x.CurrentConsultationRoomId)
                .Excluding(x => x.CurrentConsultationRoom)
                .Excluding(x => x.CurrentRoom)
                .Excluding(x => x.State)
                .Excluding(x => x.LinkedParticipants)
                .Excluding(x => x.RoomParticipants)
                .Excluding(x => x.UpdatedAt)
                .Excluding(x => x.CreatedAt)
                .Excluding(x => x.Name)
                .Excluding(x => x.HearingRole)
                .Excluding(x => x.HearingRole)
            );

            var civilianRoom = response.CivilianRooms[0];
            var room = conference.Rooms.First();
            civilianRoom.Id.Should().Be(room.Id);
            civilianRoom.Label.Should().Be(room.Label);
            civilianRoom.Participants.Select(x => x).Should()
                .BeEquivalentTo(room.RoomParticipants.Select(x => x.ParticipantId));
        }
    }
}
