using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using VideoApi.Common;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Exceptions;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using VideoApi.Events.ServiceBus;
using VideoApi.Services;

namespace VideoApi.Events.Handlers
{
    public class TransferEventHandler : EventHandlerBase
    {
        private readonly IConsultationCache _consultationCache;
        private readonly IRoomReservationService _roomReservationService;
        public TransferEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IServiceBusQueueClient serviceBusQueueClient, IConsultationCache consultationCache, IRoomReservationService roomReservationService) : base(
            queryHandler, commandHandler, serviceBusQueueClient)
        {
            _consultationCache = consultationCache;
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

            ApplicationLogger.Trace("PRIVATE_CONSULTATION", "PublishStatusAsync",
                         $"PublishStatusAsync - Conference: {SourceConference.Id}, participant : {SourceParticipant.Id}," +
                         $" Username : {SourceParticipant.Username}, EventType : {callbackEvent.EventType}, " +
                         $"TransferFrom : {callbackEvent.TransferFrom}, participantStatus : {participantStatus}, TransferTo : {callbackEvent.TransferTo}");

           
            // Alternative solution though this will only run after they  leave the room
            // if(callbackEvent.TransferFrom.GetValueOrDefault() == RoomType.ConsultationRoom1 || 
            // callbackEvent.TransferFrom.GetValueOrDefault() == RoomType.ConsultationRoom2) {
            //     roomReservationService.RemoveRoomReservation(SourceConference.Id, callbackEvent.TransferFrom);
            // }

            if(participantStatus == ParticipantState.InConsultation)
            {
                var reservationKey = $"{SourceConference.Id}:{callbackEvent.TransferTo}";
                ApplicationLogger.Trace("PRIVATE_CONSULTATION", "PublishStatusAsync",
                             $"PublishStatusAsync : reservationKey : {reservationKey}");

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
