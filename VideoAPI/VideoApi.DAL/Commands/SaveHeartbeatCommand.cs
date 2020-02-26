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
            DateTime timestamp, string browserName, string browserVersion)
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
            var @event = new Heartbeat(command.ConferenceId, command.ParticipantId, command.OutgoingAudioPercentageLost, command.OutgoingAudioPercentageLostRecent,
                command.IncomingAudioPercentageLost, command.IncomingAudioPercentageLostRecent, command.OutgoingVideoPercentageLost, 
                command.OutgoingVideoPercentageLostRecent, command.IncomingVideoPercentageLost, command.IncomingVideoPercentageLostRecent,
                command.Timestamp, command.BrowserName, command.BrowserVersion)
            {
                ParticipantId = command.ParticipantId
            };

            await _context.Heartbeats.AddAsync(@event);

            await _context.SaveChangesAsync();
        }
    }
}
