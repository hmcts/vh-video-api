using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Exceptions;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using VideoApi.Services;

namespace VideoApi.Events.Handlers
{
    public class TransferEventHandler : EventHandlerBase
    {
        private readonly IRoomReservationService _roomReservationService;
        public TransferEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IRoomReservationService roomReservationService) : base(
            queryHandler, commandHandler)
        {
            _roomReservationService = roomReservationService;
        }

        public override EventType EventType => EventType.Transfer;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var participantStatus = DeriveParticipantStatusForTransferEvent(callbackEvent);

            var command =
                new UpdateParticipantStatusAndRoomCommand(SourceConference.Id, SourceParticipant.Id, participantStatus,
                    callbackEvent.TransferTo);
            await CommandHandler.Handle(command);

            if (participantStatus == ParticipantState.InConsultation)
            {
                _roomReservationService.RemoveRoomReservation(SourceConference.Id, (RoomType)callbackEvent.TransferTo);
            }
        }

        private static ParticipantState DeriveParticipantStatusForTransferEvent(CallbackEvent callbackEvent)
        {
            if (callbackEvent.TransferFrom == RoomType.WaitingRoom &&
                (callbackEvent.TransferTo == RoomType.ConsultationRoom1 ||
                 callbackEvent.TransferTo == RoomType.ConsultationRoom2))
                return ParticipantState.InConsultation;

            if ((callbackEvent.TransferFrom == RoomType.ConsultationRoom1 ||
                 callbackEvent.TransferFrom == RoomType.ConsultationRoom2) &&
                callbackEvent.TransferTo == RoomType.WaitingRoom)
                return ParticipantState.Available;

            if ((callbackEvent.TransferFrom == RoomType.ConsultationRoom1 ||
                 callbackEvent.TransferFrom == RoomType.ConsultationRoom2) &&
                callbackEvent.TransferTo == RoomType.HearingRoom)
                return ParticipantState.InHearing;

            switch (callbackEvent.TransferFrom)
            {
                case RoomType.WaitingRoom when callbackEvent.TransferTo == RoomType.HearingRoom:
                    return ParticipantState.InHearing;
                case RoomType.HearingRoom when callbackEvent.TransferTo == RoomType.WaitingRoom:
                    return ParticipantState.Available;
                default:
                    throw new RoomTransferException(callbackEvent.TransferFrom.GetValueOrDefault(),
                        callbackEvent.TransferTo.GetValueOrDefault());
            }
        }
    }
}
