using System.Linq;
using FluentAssertions;
using FluentAssertions.Equivalency;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Consts;
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
                .Excluding(x => x.UpdatedAt)
             );

            response.StartedDateTime.Should().HaveValue().And.Be(conference.ActualStartTime);
            response.ClosedDateTime.Should().HaveValue().And.Be(conference.ClosedDateTime);
            response.CurrentStatus.Should().BeEquivalentTo(conference.GetCurrentStatus());

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
            );

            var civilianRoom = response.CivilianRooms.First();
            var room = conference.Rooms.First();
            civilianRoom.Id.Should().Be(room.Id);
            civilianRoom.Label.Should().Be(room.Label);
            civilianRoom.Participants.Select(x => x).Should()
                .BeEquivalentTo(room.RoomParticipants.Select(x => x.ParticipantId));
        }

        [Test]
        [TestCase(HearingVenueNames.Aberdeen, true)]
        [TestCase(HearingVenueNames.Dundee, true)]
        [TestCase(HearingVenueNames.Edinburgh, true)]
        [TestCase(HearingVenueNames.Glasgow, true)]
        [TestCase(HearingVenueNames.Inverness, true)]
        [TestCase(HearingVenueNames.Ayr, true)]
        [TestCase(HearingVenueNames.EdinburghEmploymentAppealTribunal, true)]
        [TestCase(HearingVenueNames.InvernessJusticeCentre, true)]
        [TestCase(HearingVenueNames.EdinburghSocialSecurityAndChildSupportTribunal, true)]
        [TestCase(HearingVenueNames.EdinburghUpperTribunal, true)]
        [TestCase("Crown Court", false)]
        [TestCase("Birmingham", false)]
        [TestCase(null, false)]
        [TestCase("", false)]
        public void Maps_Venue_Flag_Correctly(string venueName, bool expectedValue)
        {
            var conference = new ConferenceBuilder(false, null, null, venueName).Build();
            var pexipSelfTestNode = "selttest@pexip.node";

            var response = ConferenceToDetailsResponseMapper.MapConferenceToResponse(conference, pexipSelfTestNode);

            response.HearingVenueIsScottish.Should().Be(expectedValue);
        }


        bool ExcludeIdFromMessage(IMemberInfo member)
        {
            return member.SelectedMemberPath.Contains(nameof(InstantMessage)) && member.SelectedMemberInfo.Name.Contains("Id");
        }
    }
}
