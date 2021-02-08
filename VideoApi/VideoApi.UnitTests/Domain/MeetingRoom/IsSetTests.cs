using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;

namespace VideoApi.UnitTests.Domain.MeetingRoom
{
    public class IsSetTests
    {
        [Test]
        public void should_return_meeting_room_when_all_set()
        {
            // Arrange
            var conference = new ConferenceBuilder()
                .WithMeetingRoom("https://poc.node.com", "user@email.com", true)
                .Build();

            // Act/Assert
            conference.GetMeetingRoom().Should().NotBeNull();
        }

        [Test]
        public void should_return_meeting_room_when_telephone_conference_not_set()
        {
            // Arrange
            var conference = new ConferenceBuilder()
                .WithMeetingRoom("https://poc.node.com", "user@email.com", false)
                .Build();

            // Act/Assert
            conference.GetMeetingRoom().Should().NotBeNull();
        }

        [Test]
        public void should_return_null_when_pexip_node_not_set()
        {
            // Arrange
            var conference = new ConferenceBuilder()
                .WithMeetingRoom(null, "user@email.com", true)
                .Build();

            // Act/Assert
            conference.GetMeetingRoom().Should().BeNull();
        }

        [Test]
        public void should_return_meeting_room_when_conference_username_not_set()
        {
            // Arrange
            var conference = new ConferenceBuilder()
                .WithMeetingRoom("https://poc.node.com", null, true)
                .Build();

            // Act/Assert
            conference.GetMeetingRoom().Should().NotBeNull();
        }

        [Test]
        public void should_return_null_when_participant_uri_not_set()
        {
            // Arrange
            var conference = new ConferenceBuilder()
                .Build();
            conference.UpdateMeetingRoom("adminUri", "judgeUri", null, "pexipNode", "telephoneConferenceId");

            // Act/Assert
            conference.GetMeetingRoom().Should().BeNull();
        }

        [Test]
        public void should_return_null_when_judge_uri_not_set()
        {
            // Arrange
            var conference = new ConferenceBuilder()
                .Build();
            conference.UpdateMeetingRoom("adminUri", null, "participantUri", "pexipNode", "telephoneConferenceId");

            // Act/Assert
            conference.GetMeetingRoom().Should().BeNull();
        }

        [Test]
        public void should_return_null_when_admin_uri_not_set()
        {
            // Arrange
            var conference = new ConferenceBuilder()
                .Build();
            conference.UpdateMeetingRoom(null, "judgeUri", "participantUri", "pexipNode", "telephoneConferenceId");

            // Act/Assert
            conference.GetMeetingRoom().Should().BeNull();
        }
    }
}
