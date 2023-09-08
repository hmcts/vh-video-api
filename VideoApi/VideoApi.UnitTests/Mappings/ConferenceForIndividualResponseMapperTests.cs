using FluentAssertions;
using NUnit.Framework;
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
            response.HearingId.Should().Be(conference.Id);
            response.ScheduledDateTime.Should().Be(conference.ScheduledDateTime);
            response.CaseName.Should().Be(conference.CaseName);
            response.CaseNumber.Should().Be(conference.CaseNumber);
            response.Status.Should().Be(conference.State);
            response.ClosedDateTime.Should().Be(conference.ClosedDateTime);
            response.IsWaitingRoomOpen.Should().Be(conference.IsConferenceAccessible());
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

            var response = ConferenceForIndividualResponseMapper.MapConferenceSummaryToModel(conference);

            response.HearingVenueIsScottish.Should().Be(expectedValue);
        }
    }
}
