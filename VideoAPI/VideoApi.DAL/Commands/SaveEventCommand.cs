using System;
using System.Threading.Tasks;
using VideoApi.DAL.Commands.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Commands
{
    public class SaveEventCommand : ICommand
    {
        public SaveEventCommand(Guid conferenceId, string externalEventId, EventType eventType,
            DateTime externalTimestamp, RoomType? transferredFrom, RoomType? transferredTo, string reason)
        {
            ConferenceId = conferenceId;
            ExternalEventId = externalEventId;
            EventType = eventType;
            ExternalTimestamp = externalTimestamp;
            TransferredFrom = transferredFrom;
            TransferredTo = transferredTo;
            Reason = reason;
        }

        public Guid ConferenceId { get; }
        public string ExternalEventId { get; }
        public EventType EventType { get; }
        public DateTime ExternalTimestamp { get; }
        public Guid ParticipantId { get; set; }
        public RoomType? TransferredFrom { get; }
        public RoomType? TransferredTo { get; }
        public string Reason { get; }
    }

    public class SaveEventCommandHandler : ICommandHandler<SaveEventCommand>
    {
        private readonly VideoApiDbContext _context;

        public SaveEventCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(SaveEventCommand command)
        {
            var @event = new Event(command.ConferenceId, command.ExternalEventId, command.EventType,
                command.ExternalTimestamp, command.TransferredFrom, command.TransferredTo, command.Reason)
            {
                ParticipantId = command.ParticipantId
            };

            await _context.Events.AddAsync(@event);

            await _context.SaveChangesAsync();
        }
    }
}