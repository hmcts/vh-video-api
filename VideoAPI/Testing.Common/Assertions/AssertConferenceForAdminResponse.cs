using System;
using FluentAssertions;
using VideoApi.Contract.Responses;
using VideoApi.Domain.Enums;

namespace Testing.Common.Assertions
{
    public static class AssertConferenceForAdminResponse
    {
        public static void ForConference(ConferenceForAdminResponse conference)
        {
            conference.Should().NotBeNull();
            conference.CaseType.Should().NotBeNullOrEmpty();
            conference.CaseNumber.Should().NotBeNullOrEmpty();
            conference.CaseName.Should().NotBeNullOrEmpty();
            conference.ScheduledDuration.Should().BeGreaterThan(0);
            conference.ScheduledDateTime.Should().NotBe(DateTime.MinValue);
            conference.Id.Should().NotBeEmpty();
            conference.HearingRefId.Should().NotBeEmpty();
            conference.Status.Should().NotBeNull();
            conference.TelephoneConferenceNumber.Should().NotBeEmpty();
            
            if (conference.Status > ConferenceState.NotStarted && conference.Status < ConferenceState.Closed)
            {
                conference.StartedDateTime.Should().HaveValue().And.NotBe(DateTime.MinValue);
            }
            
            if (conference.Status == ConferenceState.Closed)
            {
                conference.ClosedDateTime.Should().HaveValue().And.NotBe(DateTime.MinValue);
            }

            conference.Participants.Should().NotBeNullOrEmpty();
            conference.HearingVenueName.Should().NotBeNull();
        }
    }
}
