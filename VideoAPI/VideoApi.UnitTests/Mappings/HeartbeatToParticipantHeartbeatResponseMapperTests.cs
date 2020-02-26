using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Video.API.Mappings;
using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace VideoApi.UnitTests.Mappings
{
    [TestFixture]
    public class HeartbeatToParticipantHeartbeatResponseMapperTests
    {
        [Test]
        public void Should_get_max_recent_percentage_lost()
        {
            var heartbeats = new List<Heartbeat>
            {
                new Heartbeat(Guid.Empty, Guid.Empty, 1,2,3,4,5,6,7,8, DateTime.MaxValue, "chrome", "1"),
                new Heartbeat(Guid.Empty, Guid.Empty, 8,7,6,5,4,3,2,1, DateTime.MaxValue, "chrome", "1"),
                new Heartbeat(Guid.Empty, Guid.Empty, 5456,4495,5642,9795,5653,8723,4242,3343, DateTime.MaxValue, "chrome", "1")
            };
            
            var result = HeartbeatToParticipantHeartbeatResponseMapper
                .MapHeartbeatToParticipantHeartbeatResponse(heartbeats)
                .ToList();

            result.Should().NotBeNullOrEmpty().And.NotContainNulls();
            result.Should().HaveCount(heartbeats.Count);
            result.Should().ContainItemsAssignableTo<ParticipantHeartbeatResponse>();
            result.Should().OnlyContain(x => x.BrowserName == "chrome");
            result.Should().OnlyContain(x => x.BrowserVersion == "1");
            result.Should()
                .OnlyContain(x => x.RecentPacketLoss == 8 || x.RecentPacketLoss == 7 || x.RecentPacketLoss == 9795);
        }
    }
}
