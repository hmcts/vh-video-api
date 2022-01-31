using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using VideoApi.Domain;
using VideoApi.Mappings;

namespace VideoApi.UnitTests.Mappings
{
    public class ConferenceHearingRoomsResponseMapperTests
    {
        [Test]
        public void should_map_endpoint_to_response()
        {
            List<HearingAudioRoom> hearingAudioRooms = new List<HearingAudioRoom>();

            HearingAudioRoom hearingAudioRoom1 = new HearingAudioRoom() { HearingRefId = Guid.NewGuid(), FileNamePrefix = "fileName", Label = "Label" };
            HearingAudioRoom hearingAudioRoom2 = new HearingAudioRoom() { HearingRefId = Guid.NewGuid(), FileNamePrefix = "fileName", Label = "Label" };

            hearingAudioRooms.Add(hearingAudioRoom1);
            hearingAudioRooms.Add(hearingAudioRoom2);

            var response = ConferenceHearingRoomsResponseMapper.Map(hearingAudioRooms, DateTime.UtcNow);

            response[0].HearingId.Should().Be(hearingAudioRoom1.HearingRefId.ToString());
            response[1].HearingId.Should().Be(hearingAudioRoom2.HearingRefId.ToString());

            response[0].FileNamePrefix.Should().Be("fileName");
            response[1].FileNamePrefix.Should().Be("fileName");

            response[0].Label.Should().Be("Label");
            response[1].Label.Should().Be("Label");

        }

    }
}
