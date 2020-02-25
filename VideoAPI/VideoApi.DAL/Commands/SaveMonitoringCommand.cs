using System;
using VideoApi.DAL.Commands.Core;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class SaveMonitoringCommand : ICommand
    {
        public SaveMonitoringCommand(Guid conferenceId, Guid participantId, decimal outgoingAudioPercentageLost, decimal outgoingAudioPercentageLostRecent,
            decimal incomingAudioPercentageLost, decimal incomingAudioPercentageLostRecent, decimal outgoingVideoPercentageLost, 
            decimal outgoingVideoPercentageLostRecent, decimal incomingVideoPercentageLost, decimal incomingVideoPercentageLostRecent,
            string browserName, string browserVersion)
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
        public string BrowserName { get; set; }
        public string BrowserVersion { get; set; }
    }

    public class SaveMonitoringCommandHandler : ICommandHandler<SaveMonitoringCommand>
    {
        private readonly VideoApiDbContext _context;

        public SaveMonitoringCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(SaveMonitoringCommand command)
        {
            var @event = new Monitoring(command.ConferenceId, command.ParticipantId, command.OutgoingAudioPercentageLost, command.OutgoingAudioPercentageLostRecent,
                command.IncomingAudioPercentageLost, command.IncomingAudioPercentageLostRecent, command.OutgoingVideoPercentageLost, 
                command.OutgoingVideoPercentageLostRecent, command.IncomingVideoPercentageLost, command.IncomingVideoPercentageLostRecent,
                command.BrowserName, command.BrowserVersion)
            {
                ParticipantId = command.ParticipantId
            };

            await _context.Monitoring.AddAsync(@event);

            await _context.SaveChangesAsync();
        }
    }
}
