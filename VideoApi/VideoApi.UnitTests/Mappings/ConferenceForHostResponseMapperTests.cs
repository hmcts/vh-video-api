using Testing.Common.Helper.Builders.Domain;
using VideoApi.Contract.Consts;
using VideoApi.Domain.Enums;
using VideoApi.Mappings;

namespace VideoApi.UnitTests.Mappings
{
    public class ConferenceForHostResponseMapperTests
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
            response.HearingId.Should().Be(conference.HearingRefId);
            response.ScheduledDateTime.Should().Be(conference.ScheduledDateTime);
            response.ClosedDateTime.Should().Be(conference.ClosedDateTime);
            response.ScheduledDuration.Should().Be(conference.ScheduledDuration);
            response.CaseType.Should().Be(conference.CaseType);
            response.CaseName.Should().Be(conference.CaseName);
            response.CaseNumber.Should().Be(conference.CaseNumber);
            response.Status.ToString().Should().Be(conference.State.ToString());
            response.Participants.Count.Should().Be(conference.Participants.Count);
            response.NumberOfEndpoints.Should().Be(conference.Endpoints.Count);
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

        [Test]
        [TestCase(HearingVenueNames.Aberdeen, true)]
        [TestCase(HearingVenueNames.Dundee, true)]
        [TestCase(HearingVenueNames.Edinburgh, true)]
        [TestCase(HearingVenueNames.Glasgow, true)]
        [TestCase(HearingVenueNames.Inverness, true)]
        [TestCase(HearingVenueNames.Ayr, true)]
        [TestCase("Crown Court", false)]
        [TestCase("Birmingham", false)]
        [TestCase(null, false)]
        [TestCase("", false)]
        public void Maps_Venue_Flag_Correctly(string venueName, bool expectedValue)
        {
            var conference = new ConferenceBuilder(false, null, null, venueName).Build();

            var response = ConferenceForHostResponseMapper.MapConferenceSummaryToModel(conference);

            response.HearingVenueIsScottish.Should().Be(expectedValue);
        }
    }
}
