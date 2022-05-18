using System;
using VideoApi.DAL.Commands.Core;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class SaveHeartbeatCommand : ICommand
    {
        public SaveHeartbeatCommand(Guid conferenceId, Guid participantId, decimal outgoingAudioPercentageLost, decimal outgoingAudioPercentageLostRecent,
            decimal incomingAudioPercentageLost, decimal incomingAudioPercentageLostRecent, decimal outgoingVideoPercentageLost, 
            decimal outgoingVideoPercentageLostRecent, decimal incomingVideoPercentageLost, decimal incomingVideoPercentageLostRecent,
            DateTime timestamp, string browserName, string browserVersion, string operatingSystem, string operatingSystemVersion, int outgoingAudioPacketsLost,
            string outgoingAudioBitrate, string outgoingAudioCodec, int outgoingAudioPacketSent, int outgoingVideoPacketSent, int outgoingVideoPacketsLost,
            int outgoingVideoFramerate, string outgoingVideoBitrate, string outgoingVideoCodec, string outgoingVideoResolution, string incomingAudioBitrate,
            string incomingAudioCodec, int incomingAudioPacketReceived, int incomingAudioPacketsLost, string incomingVideoBitrate, string incomingVideoCodec,
            string incomingVideoResolution, int incomingVideoPacketReceived, int incomingVideoPacketsLost)
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

        public Guid ConferenceId { get; }
        public Guid ParticipantId { get; }
        public decimal OutgoingAudioPercentageLost { get; }
        public decimal OutgoingAudioPercentageLostRecent { get; }
        public decimal IncomingAudioPercentageLost { get; }
        public decimal IncomingAudioPercentageLostRecent { get; }
        public decimal OutgoingVideoPercentageLost { get; }
        public decimal OutgoingVideoPercentageLostRecent { get; }
        public decimal IncomingVideoPercentageLost { get; }
        public decimal IncomingVideoPercentageLostRecent { get; }
        public DateTime Timestamp { get; }
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
        public string OperatingSystem { get; }
        public string OperatingSystemVersion { get; }
        public int OutgoingAudioPacketsLost { get; set; }
        public string OutgoingAudioBitrate { get; set; }
        public string OutgoingAudioCodec { get; set; }
        public int OutgoingAudioPacketSent { get; set; }
        public int OutgoingVideoPacketSent { get; set; }
        public int OutgoingVideoPacketsLost { get; set; }
        public int OutgoingVideoFramerate { get; set; }
        public string OutgoingVideoBitrate { get; set; }
        public string OutgoingVideoCodec { get; set; }
        public string OutgoingVideoResolution { get; set; }
        public string IncomingAudioBitrate { get; set; }
        public string IncomingAudioCodec { get; set; }
        public int IncomingAudioPacketReceived { get; set; }
        public int IncomingAudioPacketsLost { get; set; }
        public string IncomingVideoBitrate { get; set; }
        public string IncomingVideoCodec { get; set; }
        public string IncomingVideoResolution { get; set; }
        public int IncomingVideoPacketReceived { get; set; }
        public int IncomingVideoPacketsLost { get; set; }
    }

    public class SaveHeartbeatCommandHandler : ICommandHandler<SaveHeartbeatCommand>
    {
        private readonly VideoApiDbContext _context;

        public SaveHeartbeatCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(SaveHeartbeatCommand command)
        {
            var @event = new Heartbeat(command.ConferenceId, command.ParticipantId, command.OutgoingAudioPercentageLost,
                command.OutgoingAudioPercentageLostRecent, command.IncomingAudioPercentageLost,
                command.IncomingAudioPercentageLostRecent, command.OutgoingVideoPercentageLost,
                command.OutgoingVideoPercentageLostRecent, command.IncomingVideoPercentageLost,
                command.IncomingVideoPercentageLostRecent, command.Timestamp, command.BrowserName,
                command.BrowserVersion, command.OperatingSystem, command.OperatingSystemVersion, 
                command.OutgoingAudioPacketsLost, command.OutgoingAudioBitrate, command.OutgoingAudioCodec,
                command.OutgoingAudioPacketSent, command.OutgoingVideoPacketSent, command.OutgoingVideoPacketsLost,
                command.OutgoingVideoFramerate, command.OutgoingVideoBitrate, command.OutgoingVideoCodec,
                command.OutgoingVideoResolution, command.IncomingAudioBitrate, command.IncomingAudioCodec, 
                command.IncomingAudioPacketReceived, command.IncomingAudioPacketsLost, command.IncomingVideoBitrate,
                command.IncomingVideoCodec, command.IncomingVideoResolution, command.IncomingVideoPacketReceived,
                command.IncomingVideoPacketsLost)
            {
                ParticipantId = command.ParticipantId
            };

            await _context.Heartbeats.AddAsync(@event);

            await _context.SaveChangesAsync();
        }
    }
}
