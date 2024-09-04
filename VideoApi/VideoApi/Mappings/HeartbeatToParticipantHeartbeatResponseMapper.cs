using System.Collections.Generic;
using System.Linq;
using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace VideoApi.Mappings
{
    public static class HeartbeatToParticipantHeartbeatResponseMapper
    {
        public static IEnumerable<ParticipantHeartbeatResponse> MapHeartbeatToParticipantHeartbeatResponse(
            IEnumerable<Heartbeat> heartbeat)
        {
            return heartbeat.Select(x => new ParticipantHeartbeatResponse
            {
                RecentPacketLoss = GetMaxPercentageLostRecent(x),
                BrowserName = x.BrowserName,
                BrowserVersion = x.BrowserVersion,
                OperatingSystem = x.OperatingSystem,
                OperatingSystemVersion = x.OperatingSystemVersion,
                Timestamp = x.Timestamp
            });
        }
        
        private static decimal GetMaxPercentageLostRecent(Heartbeat heartbeat)
        {
            return new[]
            {
                heartbeat.IncomingAudioPercentageLostRecent,
                heartbeat.IncomingVideoPercentageLostRecent,
                heartbeat.OutgoingAudioPercentageLostRecent,
                heartbeat.OutgoingVideoPercentageLostRecent,
            }.Max();
        }
    }
}
