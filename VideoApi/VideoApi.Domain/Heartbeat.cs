using System;
using VideoApi.Domain.Ddd;

namespace VideoApi.Domain
{
    public class Heartbeat : Entity<long>
    {
        public Guid ConferenceId { get; set; }
        public Guid ParticipantId { get; set; }
        public decimal OutgoingAudioPercentageLost { get; set; }
        public decimal OutgoingAudioPercentageLostRecent { get; set; }
        public decimal IncomingAudioPercentageLost { get; set; }
        public decimal IncomingAudioPercentageLostRecent { get; set; }
        public decimal OutgoingVideoPercentageLost { get; set; }
        public decimal OutgoingVideoPercentageLostRecent { get; set; }
        public decimal IncomingVideoPercentageLost { get; set; }
        public decimal IncomingVideoPercentageLostRecent { get; set; }
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
        public string OperatingSystem { get; }
        public string OperatingSystemVersion { get; }
        public DateTime Timestamp { get; set; }
        public int? OutgoingAudioPacketsLost { get; set; }
        public string OutgoingAudioBitrate { get; set; }
        public string OutgoingAudioCodec { get; set; }
        public int? OutgoingAudioPacketSent { get; set; }
        public int? OutgoingVideoPacketSent { get; set; }
        public int? OutgoingVideoPacketsLost { get; set; }
        public int? OutgoingVideoFramerate { get; set; }
        public string OutgoingVideoBitrate { get; set; }
        public string OutgoingVideoCodec { get; set; }
        public string OutgoingVideoResolution { get; set; }
        public string IncomingAudioBitrate { get; set; }
        public string IncomingAudioCodec { get; set; }
        public int? IncomingAudioPacketReceived { get; set; }
        public int? IncomingAudioPacketsLost { get; set; }
        public string IncomingVideoBitrate { get; set; }
        public string IncomingVideoCodec { get; set; }
        public string IncomingVideoResolution { get; set; }
        public int? IncomingVideoPacketReceived { get; set; }
        public int? IncomingVideoPacketsLost { get; set; }


        public Heartbeat(Guid conferenceId, Guid participantId, decimal outgoingAudioPercentageLost, decimal outgoingAudioPercentageLostRecent,
            decimal incomingAudioPercentageLost, decimal incomingAudioPercentageLostRecent, decimal outgoingVideoPercentageLost, 
            decimal outgoingVideoPercentageLostRecent, decimal incomingVideoPercentageLost, decimal incomingVideoPercentageLostRecent,
            DateTime timestamp, string browserName, string browserVersion, string operatingSystem, string operatingSystemVersion, int? outgoingAudioPacketsLost,
            string outgoingAudioBitrate, string outgoingAudioCodec, int? outgoingAudioPacketSent, int? outgoingVideoPacketSent, int? outgoingVideoPacketsLost, 
            int? outgoingVideoFramerate, string outgoingVideoBitrate, string outgoingVideoCodec, string outgoingVideoResolution, string incomingAudioBitrate,
            string incomingAudioCodec, int? incomingAudioPacketReceived, int? incomingAudioPacketsLost, string incomingVideoBitrate, string incomingVideoCodec,
            string incomingVideoResolution, int? incomingVideoPacketReceived, int? incomingVideoPacketsLost)
        {
            ConferenceId = conferenceId;
            ParticipantId = participantId;
            OutgoingAudioPercentageLost = outgoingAudioPercentageLost;
            OutgoingAudioPercentageLostRecent = outgoingAudioPercentageLostRecent;
            IncomingAudioPercentageLost = incomingAudioPercentageLost;
            IncomingAudioPercentageLostRecent = incomingAudioPercentageLostRecent;
            OutgoingVideoPercentageLost = outgoingVideoPercentageLost;
            OutgoingVideoPercentageLostRecent = outgoingVideoPercentageLostRecent;
            IncomingVideoPercentageLost = incomingVideoPercentageLost;
            IncomingVideoPercentageLostRecent = incomingVideoPercentageLostRecent;
            Timestamp = timestamp;
            BrowserName = browserName;
            BrowserVersion = browserVersion;
            OperatingSystem = operatingSystem;
            OperatingSystemVersion = operatingSystemVersion;
            OutgoingAudioPacketsLost = outgoingAudioPacketsLost;
            OutgoingAudioBitrate = outgoingAudioBitrate;
            OutgoingAudioCodec = outgoingAudioCodec;
            OutgoingAudioPacketSent = outgoingAudioPacketSent;
            OutgoingVideoPacketSent = outgoingVideoPacketSent;
            OutgoingVideoPacketsLost = outgoingVideoPacketsLost;
            OutgoingVideoFramerate = outgoingVideoFramerate;
            OutgoingVideoBitrate = outgoingVideoBitrate;
            OutgoingVideoCodec = outgoingVideoCodec;
            OutgoingVideoResolution = outgoingVideoResolution;
            IncomingAudioBitrate = incomingAudioBitrate;
            IncomingAudioCodec = incomingAudioCodec;
            IncomingAudioPacketReceived = incomingAudioPacketReceived;
            IncomingAudioPacketsLost = incomingAudioPacketsLost;
            IncomingVideoBitrate = incomingVideoBitrate;
            IncomingVideoCodec = incomingVideoCodec;
            IncomingVideoResolution = incomingVideoResolution;
            IncomingVideoPacketReceived = incomingVideoPacketReceived;
            IncomingVideoPacketsLost = incomingVideoPacketsLost;
        }
    }
}
