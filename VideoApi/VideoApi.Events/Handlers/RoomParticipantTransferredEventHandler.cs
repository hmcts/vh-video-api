using Microsoft.Extensions.Logging;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Exceptions;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Events.Handlers
{
    public class RoomParticipantTransferredEventHandler : EventHandlerBase<RoomParticipantTransferredEventHandler>
    {
        public RoomParticipantTransferredEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler,
            ILogger<RoomParticipantTransferredEventHandler> logger) : base(queryHandler, commandHandler, logger)
        {
        }

        public override EventType EventType => EventType.RoomParticipantTransfer;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var i = 0;
            foreach (var participant in SourceParticipantRoom.RoomParticipants)
            {
                i++;
                var participantStatus = DeriveParticipantStatusForTransferEvent(callbackEvent);
                _logger.LogInformation("Room Participant Transferred ({Iteration}) callback received - {ConferenceId}/{ParticipantId} - {FromRoom} {FromRoomLabel} - {ToRoom} {ToRoomLabel} - {NewStatus}",
                     i, SourceConference.Id, participant.Id, callbackEvent.TransferFrom, callbackEvent.TransferredFromRoomLabel, callbackEvent.TransferTo, callbackEvent.TransferredToRoomLabel, participantStatus);

                var command =
                    new UpdateParticipantStatusAndRoomCommand(SourceConference.Id, participant.ParticipantId, participantStatus,
                        callbackEvent.TransferTo, callbackEvent.TransferredToRoomLabel);
                await CommandHandler.Handle(command);
            }
        }
        
        private ParticipantState DeriveParticipantStatusForTransferEvent(CallbackEvent callbackEvent)
        {
            if (!callbackEvent.TransferTo.HasValue && callbackEvent.TransferredToRoomLabel.ToLower().Contains("consultation"))
            {
                return ParticipantState.InConsultation;
            }

            switch (callbackEvent.TransferTo)
            {
                case RoomType.ConsultationRoom:
                    return ParticipantState.InConsultation;
                case RoomType.WaitingRoom:
                    return ParticipantState.Available;
                case RoomType.HearingRoom:
                    return ParticipantState.InHearing;
                default:
                    throw new RoomTransferException(callbackEvent.TransferredFromRoomLabel,
                        callbackEvent.TransferredToRoomLabel);
            }
        }
    }
}
