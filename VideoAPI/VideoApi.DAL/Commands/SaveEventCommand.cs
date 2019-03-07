using System;
using System.Threading.Tasks;
using VideoApi.DAL.Commands.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Commands
{
    public class SaveEventCommand : ICommand
    {
        public SaveEventCommand(string externalEventId, EventType eventType, DateTime externalTimestamp,
            Guid participantId, RoomType? transferredFrom, RoomType? transferredTo, string reason)
        {
            ExternalEventId = externalEventId;
            EventType = eventType;
            ExternalTimestamp = externalTimestamp;
            ParticipantId = participantId;
            TransferredFrom = transferredFrom;
            TransferredTo = transferredTo;
            Reason = reason;
        }
        public string ExternalEventId { get; set; }
        public EventType EventType { get; set; }
        public DateTime ExternalTimestamp { get; set; }
        public Guid ParticipantId { get; set; }
        public RoomType? TransferredFrom { get; set; }
        public RoomType? TransferredTo { get; set; }
        public string Reason { get; set; }
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
            var @event = new Event(command.ExternalEventId, command.EventType, command.ExternalTimestamp,
                command.ParticipantId, command.TransferredFrom, command.TransferredTo, command.Reason);

            await _context.Events.AddAsync(@event);
            
            await _context.SaveChangesAsync(); 
        }
    }
}