using System;
using VideoApi.Common;
using VideoApi.DAL.Commands.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class SaveEventCommand(
        Guid conferenceId,
        string externalEventId,
        EventType eventType,
        DateTime externalTimestamp,
        RoomType? transferredFrom,
        RoomType? transferredTo,
        string reason,
        string phone)
        : ICommand
    {
        public Guid ConferenceId { get; } = conferenceId;
        public string ExternalEventId { get; } = externalEventId;
        public EventType EventType { get; } = eventType;
        public DateTime ExternalTimestamp { get; } = externalTimestamp;
        public Guid ParticipantId { get; set; }
        public RoomType? TransferredFrom { get; } = transferredFrom;
        public RoomType? TransferredTo { get; } = transferredTo;
        public string Reason { get; } = reason;
        public bool IsEndpoint { get; } = eventType.IsEndpointEvent();
        public string Phone { get; } = phone;
        public string TransferredFromRoomLabel { get; set; }
        public string TransferredToRoomLabel { get; set; }
        public long? ParticipantRoomId { get; set; }
    }

    public class SaveEventCommandHandler(VideoApiDbContext context) : ICommandHandler<SaveEventCommand>
    {
        public async Task Handle(SaveEventCommand command)
        {
            var @event = MapCommandToEvent(command);
            await context.Events.AddAsync(@event);
            await context.SaveChangesAsync();
        }

        private static Event MapCommandToEvent(SaveEventCommand command)
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
