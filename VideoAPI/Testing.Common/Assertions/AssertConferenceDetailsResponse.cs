using System;
using FluentAssertions;
using VideoApi.Contract.Responses;
using VideoApi.Domain.Enums;

namespace Testing.Common.Assertions
{
    public class AssertConferenceDetailsResponse
    {
        protected AssertConferenceDetailsResponse()
        {
        }

        public static void ForConference(ConferenceDetailsResponse conference)
        {
            conference.Should().NotBeNull();
            conference.CaseType.Should().NotBeNullOrEmpty();
            conference.CaseNumber.Should().NotBeNullOrEmpty();
            conference.CaseName.Should().NotBeNullOrEmpty();
            conference.ScheduledDuration.Should().BeGreaterThan(0);
            conference.ScheduledDateTime.Should().NotBe(DateTime.MinValue);
            conference.CurrentStatus.Should().NotBeNull();

            foreach (var participant in conference.Participants)
            {
                participant.Id.Should().NotBeEmpty();
                participant.Name.Should().NotBeNullOrEmpty();
                participant.DisplayName.Should().NotBeNullOrEmpty();
                participant.Username.Should().NotBeNullOrEmpty();
                participant.UserRole.Should().NotBe(UserRole.None);
                participant.CaseTypeGroup.Should().NotBeNullOrEmpty();
                participant.CurrentStatus.Should().NotBe(ParticipantState.None);
            }

            conference.MeetingRoom.Should().NotBeNull();
            conference.MeetingRoom.AdminUri.Should().NotBeNull();
            conference.MeetingRoom.JudgeUri.Should().NotBeNull();
            conference.MeetingRoom.ParticipantUri.Should().NotBeNull();
            conference.MeetingRoom.PexipNode.Should().NotBeNull();
        }
    }
}
