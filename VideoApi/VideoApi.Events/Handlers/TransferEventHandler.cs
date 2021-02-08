using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Exceptions;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;

namespace VideoApi.Events.Handlers
{
    public class TransferEventHandler : EventHandlerBase<TransferEventHandler>
    {
        public TransferEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<TransferEventHandler> logger) : base(
            queryHandler, commandHandler, logger)
        {
        }

        public override EventType EventType => EventType.Transfer;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var participantStatus = DeriveParticipantStatusForTransferEvent(callbackEvent);

            var command =
                new UpdateParticipantStatusAndRoomCommand(SourceConference.Id, SourceParticipant.Id, participantStatus,
                    callbackEvent.TransferTo, callbackEvent.TransferredToRoomLabel);
            await CommandHandler.Handle(command);
        }

        private ParticipantState DeriveParticipantStatusForTransferEvent(CallbackEvent callbackEvent)
        {
            if (!callbackEvent.TransferTo.HasValue && callbackEvent.TransferredToRoomLabel.ToLower().Contains("consultation"))
            {
                return ParticipantState.InConsultation;
            }
            
            
            if (callbackEvent.TransferFrom == RoomType.WaitingRoom &&
                callbackEvent.TransferTo == RoomType.ConsultationRoom)
                return ParticipantState.InConsultation;

            if ((callbackEvent.TransferFrom == RoomType.ConsultationRoom || 
                 callbackEvent.TransferredFromRoomLabel.ToLower().Contains("consultation")) &&
                callbackEvent.TransferTo == RoomType.WaitingRoom)
                return ParticipantState.Available;

            if (callbackEvent.TransferFrom == RoomType.ConsultationRoom &&
                callbackEvent.TransferTo == RoomType.HearingRoom)
                return ParticipantState.InHearing;

            switch (callbackEvent.TransferFrom)
            {
                case RoomType.WaitingRoom when callbackEvent.TransferTo == RoomType.HearingRoom:
                    return ParticipantState.InHearing;
                case RoomType.HearingRoom when callbackEvent.TransferTo == RoomType.WaitingRoom:
                    return ParticipantState.Available;
                default:
                    throw new RoomTransferException(callbackEvent.TransferredFromRoomLabel,
                        callbackEvent.TransferredToRoomLabel);
            }
        }
    }
}
