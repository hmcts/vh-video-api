using FluentAssertions;
using System;
using VideoApi.Contract.Responses;

namespace Testing.Common.Assertions
{
    public static class AssertConferenceForJudgeResponse
    {
        public static void ForConference(ConferenceForJudgeResponse conference)
        {
            conference.Should().NotBeNull();
            conference.CaseType.Should().NotBeNullOrEmpty();
            conference.CaseNumber.Should().NotBeNullOrEmpty();
            conference.CaseName.Should().NotBeNullOrEmpty();
            conference.ScheduledDuration.Should().BeGreaterThan(0);
            conference.ScheduledDateTime.Should().NotBe(DateTime.MinValue);
            conference.Id.Should().NotBeEmpty();
            conference.Status.Should().NotBeNull();
            conference.NumberOfEndpoints.Should().BeGreaterThan(-1);
            conference.Participants.Should().NotBeNullOrEmpty();
        }
    }
}
