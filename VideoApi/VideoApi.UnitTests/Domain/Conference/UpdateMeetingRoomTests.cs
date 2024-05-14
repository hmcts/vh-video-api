using Testing.Common.Helper.Builders.Domain;

namespace VideoApi.UnitTests.Domain.Conference
{
    public class UpdateMeetingRoomTests
    {
        [Test]
        public void Should_not_have_room_defined_by_default()
        {
            var conference = new ConferenceBuilder().Build();
            conference.GetMeetingRoom().Should().BeNull();
        }

        [Test]
        public void Should_update_existing_room_details()
        {
            var conference = new ConferenceBuilder().WithMeetingRoom("poc.node.com", "user@hmcts.net").Build();
            conference.GetMeetingRoom().Should().NotBeNull();

            const string adminUri = "https://testpoc.node.com/viju/#/?conference=user@hmcts.net&output=embed";
            const string judgeUri = "https://testpoc.node.com/viju/#/?conference=user@hmcts.net&output=embed";
            const string participantUri = "https://testpoc.node.com/viju/#/?conference=user@hmcts.net&output=embed";
            const string pexipNode = "testpoc.node.com";
            const string telephoneConferenceId = "12345678";
            conference.UpdateMeetingRoom(adminUri, judgeUri, participantUri, pexipNode, telephoneConferenceId);

            conference.GetMeetingRoom().Should().NotBeNull();
            var meetingRoom = conference.GetMeetingRoom();
            meetingRoom.AdminUri.Should().Be(adminUri);
            meetingRoom.JudgeUri.Should().Be(judgeUri);
            meetingRoom.ParticipantUri.Should().Be(participantUri);
            meetingRoom.PexipNode.Should().Be(pexipNode);
        }

        [Test]
        public void Should_add_room_details()
        {
            var conference = new ConferenceBuilder().Build();
            conference.GetMeetingRoom().Should().BeNull();

            const string adminUri = "https://poc.node.com/viju/#/?conference=user@hmcts.net&output=embed";
            const string judgeUri = "https://poc.node.com/viju/#/?conference=user@hmcts.net&output=embed";
            const string participantUri = "https://poc.node.com/viju/#/?conference=user@hmcts.net&output=embed";
            const string pexipNode = "poc.node.com";
            const string telephoneConferenceId = "12345678";
            conference.UpdateMeetingRoom(adminUri, judgeUri, participantUri, pexipNode, telephoneConferenceId);

            conference.GetMeetingRoom().Should().NotBeNull();
            var meetingRoom = conference.GetMeetingRoom();
            meetingRoom.AdminUri.Should().Be(adminUri);
            meetingRoom.JudgeUri.Should().Be(judgeUri);
            meetingRoom.ParticipantUri.Should().Be(participantUri);
            meetingRoom.PexipNode.Should().Be(pexipNode);
        }
    }
}
