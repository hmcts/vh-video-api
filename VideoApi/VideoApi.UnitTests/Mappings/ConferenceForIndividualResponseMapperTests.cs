using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Consts;
using VideoApi.Domain.Enums;
using VideoApi.Mappings;

namespace VideoApi.UnitTests.Mappings
{
    public class ConferenceForIndividualResponseMapperTests
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
                .Build();

            var response = ConferenceForIndividualResponseMapper.MapConferenceSummaryToModel(conference);
            
            response.Id.Should().Be(conference.Id);
            response.HearingId.Should().Be(conference.HearingRefId);
            response.ScheduledDateTime.Should().Be(conference.ScheduledDateTime);
            response.CaseName.Should().Be(conference.CaseName);
            response.CaseNumber.Should().Be(conference.CaseNumber);
            response.Status.Should().Be((Contract.Enums.ConferenceState)conference.State);
            response.ClosedDateTime.Should().Be(conference.ClosedDateTime);
            response.IsWaitingRoomOpen.Should().Be(conference.IsConferenceAccessible());
        }
    }
}
