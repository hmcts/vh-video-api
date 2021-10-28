using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Mappings;

namespace VideoApi.UnitTests.Mappings
{
    public class ConferenceForJudgeResponseMapperTests
    {
        [Test]
        public void should_map_all_properties()
        {
            var conference = new ConferenceBuilder()
                .WithConferenceStatus(ConferenceState.InSession)
                .WithConferenceStatus(ConferenceState.Paused)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithParticipant(UserRole.Judge, "Judge")
                .WithParticipants(3)
                .WithEndpoint("3000","sip address")
                .Build();

            var response = ConferenceForHostResponseMapper.MapConferenceSummaryToModel(conference);
            
            response.Id.Should().Be(conference.Id);
            response.ScheduledDateTime.Should().Be(conference.ScheduledDateTime);
            response.ClosedDateTime.Should().Be(conference.ClosedDateTime);
            response.ScheduledDuration.Should().Be(conference.ScheduledDuration);
            response.CaseType.Should().Be(conference.CaseType);
            response.CaseName.Should().Be(conference.CaseName);
            response.CaseNumber.Should().Be(conference.CaseNumber);
            response.Status.ToString().Should().Be(conference.State.ToString());
            response.Participants.Count.Should().Be(conference.Participants.Count);
            response.NumberOfEndpoints.Should().Be(conference.Endpoints.Count);
            response.HearingVenueName.Should().Be(conference.HearingVenueName);
        }

        [Test]
        public void should_map_number_of_endpoints_to_zero()
        {
            var conference = new ConferenceBuilder()
                .WithConferenceStatus(ConferenceState.InSession)
                .WithParticipant(UserRole.Judge, "Judge")
                .WithParticipants(1)
                .Build();

            var response = ConferenceForHostResponseMapper.MapConferenceSummaryToModel(conference);

            response.Id.Should().Be(conference.Id);
            response.NumberOfEndpoints.Should().Be(0);
        }
    }
}
