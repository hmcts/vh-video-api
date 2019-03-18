using System;
using FluentAssertions;
using VideoApi.Contract.Responses;
using VideoApi.Domain.Enums;

namespace Testing.Common.Assertions
{
    public class AssertConferenceSummaryResponse
    {
        protected AssertConferenceSummaryResponse()
        {
        }

        public static void ForConference(ConferenceSummaryResponse conference)
        {
            conference.Should().NotBeNull();
            conference.CaseType.Should().NotBeNullOrEmpty();
            conference.CaseNumber.Should().NotBeNullOrEmpty();
            conference.CaseName.Should().NotBeNullOrEmpty();
            conference.ScheduledDateTime.Should().NotBe(DateTime.MinValue);
            conference.Id.Should().NotBeEmpty();
        }
    }
}
