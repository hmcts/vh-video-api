using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class UpdateMeetingRoomTests
    {
        [Test]
        public void should_not_have_room_defined_by_default()
        {
            var conference = new ConferenceBuilder().Build();
            conference.GetMeetingRoom().Should().BeNull();
        }
        
        [Test]
        public void should_update_existing_room_details()
        {
            var conference = new ConferenceBuilder().WithMeetingRoom().Build();
            conference.GetMeetingRoom().Should().NotBeNull();
            
            var adminUri = "https://testjoin.poc.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var judgeUri = "https://testjoin.poc.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var participantUri =
                "https://testjoin.poc.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var pexipNode = "testjoin.poc.hearings.hmcts.net";
            conference.UpdateMeetingRoom(adminUri, judgeUri, participantUri, pexipNode);
            
            conference.GetMeetingRoom().Should().NotBeNull();
            var meetingRoom = conference.GetMeetingRoom();
            meetingRoom.AdminUri.Should().Be(adminUri);
            meetingRoom.JudgeUri.Should().Be(judgeUri);
            meetingRoom.ParticipantUri.Should().Be(participantUri);
            meetingRoom.PexipNode.Should().Be(pexipNode);
        }
        
        [Test]
        public void should_add_room_details()
        {
            var conference = new ConferenceBuilder().Build();
            conference.GetMeetingRoom().Should().BeNull();
            
            var adminUri = "https://join.poc.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var judgeUri = "https://join.poc.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var participantUri =
                "https://join.poc.hearings.hmcts.net/viju/#/?conference=ola@hearings.hmcts.net&output=embed";
            var pexipNode = "join.poc.hearings.hmcts.net";
            conference.UpdateMeetingRoom(adminUri, judgeUri, participantUri, pexipNode);

            conference.GetMeetingRoom().Should().NotBeNull();
            var meetingRoom = conference.GetMeetingRoom();
            meetingRoom.AdminUri.Should().Be(adminUri);
            meetingRoom.JudgeUri.Should().Be(judgeUri);
            meetingRoom.ParticipantUri.Should().Be(participantUri);
            meetingRoom.PexipNode.Should().Be(pexipNode);
        }
    }
}