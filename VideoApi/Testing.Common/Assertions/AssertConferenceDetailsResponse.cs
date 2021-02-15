using System;
using FluentAssertions;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Responses;

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
            conference.HearingVenueName.Should().NotBeNull();

            conference.Participants.Should().NotBeNullOrEmpty();
            foreach (var participant in conference.Participants)
            {
                participant.Name.Should().NotBeNullOrEmpty();
                participant.DisplayName.Should().NotBeNullOrEmpty();
                participant.Username.Should().NotBeNullOrEmpty();
                participant.UserRole.Should().NotBe(UserRole.None);
                participant.CaseTypeGroup.Should().NotBeNullOrEmpty();
                participant.HearingRole.Should().NotBeNullOrEmpty();
                participant.FirstName.Should().NotBeNullOrWhiteSpace();
                participant.LastName.Should().NotBeNullOrWhiteSpace();
                participant.ContactEmail.Should().NotBeNullOrWhiteSpace();
                participant.CurrentStatus.Should().NotBe(ParticipantState.None);
                if (participant.UserRole == UserRole.Representative)
                {
                    participant.Representee.Should().NotBeNullOrEmpty();
                }
            }

            conference.MeetingRoom.Should().NotBeNull();
            conference.MeetingRoom.AdminUri.Should().NotBeNull();
            conference.MeetingRoom.JudgeUri.Should().NotBeNull();
            conference.MeetingRoom.ParticipantUri.Should().NotBeNull();
            conference.MeetingRoom.PexipNode.Should().NotBeNull();
            conference.MeetingRoom.TelephoneConferenceId.Should().NotBeNull();
            conference.MeetingRoom.PexipSelfTestNode.Should().NotBeNullOrEmpty();

            if (conference.CurrentStatus > ConferenceState.NotStarted)
            {
                conference.StartedDateTime.Should().HaveValue().And.NotBe(DateTime.MinValue);
            }
            
            if (conference.CurrentStatus == ConferenceState.Closed)
            {
                conference.ClosedDateTime.Should().HaveValue().And.NotBe(DateTime.MinValue);
            }

            conference.AudioRecordingRequired.Should().BeFalse();
            
            foreach (var endpoint in conference.Endpoints)
            {
                endpoint.Id.Should().NotBeEmpty();
                endpoint.DisplayName.Should().NotBeNullOrWhiteSpace();
                endpoint.SipAddress.Should().NotBeNullOrWhiteSpace();
                endpoint.Pin.Should().NotBeNullOrWhiteSpace();
            }
        }

        public static void ForConferenceEndpoints(ConferenceDetailsResponse conference)
        {
            conference.Endpoints.Should().NotBeNullOrEmpty();
            foreach (var endpoint in conference.Endpoints)
            {
                endpoint.Id.Should().NotBeEmpty();
                endpoint.DisplayName.Should().NotBeEmpty();
                endpoint.SipAddress.Should().NotBeEmpty();
                endpoint.Pin.Should().NotBeEmpty();
            }
        }
    }
}
