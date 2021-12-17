using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using Testing.Common.Helper.Builders.Domain;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Mappings;

namespace VideoApi.UnitTests.Mappings
{
    public class ConferenceHearingRoomsResponseMapperTests
    {
        [Test]
        public void should_map_endpoint_to_response()
        {
            var conference1 = new ConferenceBuilder()
                .WithConferenceStatus(ConferenceState.InSession, DateTime.UtcNow.Date)
                .WithConferenceStatus(ConferenceState.InSession, DateTime.UtcNow.Date)
                .WithConferenceStatus(ConferenceState.Closed)
                .WithParticipant(UserRole.Judge, "Judge")
                .WithParticipants(3)
                .Build();
            List<Conference> conferences = new List<Conference> { conference1 };
            var response = ConferenceHearingRoomsResponseMapper.Map(conferences, DateTime.UtcNow);
            
            response[0].ConferenceState.Should().Be(ConferenceState.InSession);
            response[1].ConferenceState.Should().Be(ConferenceState.InSession);
         
            response[0].TimeStamp.Should().Be(DateTime.UtcNow.Date.ToString("O"));
            response[1].TimeStamp.Should().Be(DateTime.UtcNow.Date.ToString("O"));
            
            response[0].HearingId.Should().Be(conference1.HearingRefId.ToString());
            response[1].HearingId.Should().Be(conference1.HearingRefId.ToString());

        }
    }
}
