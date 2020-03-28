using System;
using FluentAssertions;
using VideoApi.Contract.Responses;

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
        }
    }
}
