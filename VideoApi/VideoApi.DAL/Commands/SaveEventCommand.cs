using System;
using VideoApi.Common;
using VideoApi.DAL.Commands.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class SaveEventCommand : ICommand
    {
        public SaveEventCommand(Guid conferenceId, string externalEventId, EventType eventType,
            DateTime externalTimestamp, RoomType? transferredFrom, RoomType? transferredTo, string reason, string phone)
        {
            ConferenceId = conferenceId;
            ExternalEventId = externalEventId;
            EventType = eventType;
            ExternalTimestamp = externalTimestamp;
            TransferredFrom = transferredFrom;
            TransferredTo = transferredTo;
            Reason = reason;
            IsEndpoint = eventType.IsEndpointEvent();
            Phone = phone;
        }

        public Guid ConferenceId { get; }
        public string ExternalEventId { get; }
        public EventType EventType { get; }
        public DateTime ExternalTimestamp { get; }
        public Guid ParticipantId { get; set; }
        public RoomType? TransferredFrom { get; }
        public RoomType? TransferredTo { get; }
        public string Reason { get; }
        public bool IsEndpoint { get; }
        public string Phone { get; }
        public string TransferredFromRoomLabel { get; set; }
        public string TransferredToRoomLabel { get; set; }
        public long? ParticipantRoomId { get; set; }
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
            var @event = MapCommandToEvent(command);
            await _context.Events.AddAsync(@event);
            await _context.SaveChangesAsync();
        }

        private Event MapCommandToEvent(SaveEventCommand command)
        {
            return new Event(command.ConferenceId, command.ExternalEventId, command.EventType,
                command.ExternalTimestamp, command.TransferredFrom, command.TransferredTo, command.Reason, command.Phone)
            {
                ParticipantId = command.ParticipantId,
                EndpointFlag = command.IsEndpoint,
                TransferredFromRoomLabel = command.TransferredFromRoomLabel,
                TransferredToRoomLabel = command.TransferredToRoomLabel,
                ParticipantRoomId = command.ParticipantRoomId
            };
        }
    }
}
