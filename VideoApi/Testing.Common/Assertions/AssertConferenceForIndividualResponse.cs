using FluentAssertions;
using System;
using VideoApi.Contract.Responses;

namespace Testing.Common.Assertions
{
    public static class AssertConferenceForIndividualResponse
    {
        public static void ForConference(ConferenceForIndividualResponse conference)
        {
            conference.Should().NotBeNull();
            conference.CaseNumber.Should().NotBeNullOrEmpty();
            conference.CaseName.Should().NotBeNullOrEmpty();
            conference.ScheduledDateTime.Should().NotBe(DateTime.MinValue);
            conference.Id.Should().NotBeEmpty();
            conference.HearingId.Should().NotBeEmpty();
            conference.Status.Should().Be(conference.Status);
            conference.ClosedDateTime.Should().NotBe(DateTime.MinValue);
            conference.IsWaitingRoomOpen.Should().BeTrue();
        }
    }
}
