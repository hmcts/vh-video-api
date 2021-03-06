using System.Linq;
using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain;
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
                .Excluding(x => ExcludeIdFromMessage(x))
                .Excluding(x => x.IngestUrl)
                .Excluding(x => x.ActualStartTime)
                .Excluding(x => x.Endpoints)
                .Excluding(x => x.CreatedDateTime)
                .Excluding(x => x.Rooms)
             );

            response.StartedDateTime.Should().HaveValue().And.Be(conference.ActualStartTime);
            response.ClosedDateTime.Should().HaveValue().And.Be(conference.ClosedDateTime);
            response.CurrentStatus.Should().BeEquivalentTo(conference.GetCurrentStatus());

            var participants = conference.GetParticipants();
            response.Participants.Should().BeEquivalentTo(participants, options => options
                .Excluding(x => x.ParticipantRefId)
                .Excluding(x => x.TestCallResultId)
                .Excluding(x => x.TestCallResult)
                .Excluding(x => x.CurrentConsultationRoomId)
                .Excluding(x => x.CurrentConsultationRoom)
                .Excluding(x => x.CurrentRoom)
                .Excluding(x => x.State)
                .Excluding(x => x.LinkedParticipants)
                .Excluding(x => x.RoomParticipants)
            );

            var civilianRoom = response.CivilianRooms.First();
            var room = conference.Rooms.First();
            civilianRoom.Id.Should().Be(room.Id);
            civilianRoom.Label.Should().Be(room.Label);
            civilianRoom.Participants.Select(x => x).Should()
                .BeEquivalentTo(room.RoomParticipants.Select(x => x.ParticipantId));
        }

        
        bool ExcludeIdFromMessage(IMemberInfo member)
        {
            return member.SelectedMemberPath.Contains(nameof(InstantMessage)) && member.SelectedMemberInfo.Name.Contains("Id");
        }
    }
}
