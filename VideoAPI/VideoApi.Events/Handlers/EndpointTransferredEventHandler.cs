using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Exceptions;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using VideoApi.Services.Contracts;

namespace VideoApi.Events.Handlers
{
    public class EndpointTransferredEventHandler : EventHandlerBase
    {
        private readonly IRoomReservationService _roomReservationService;
        public EndpointTransferredEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, IRoomReservationService roomReservationService) : base(queryHandler, commandHandler)
        {
            _roomReservationService = roomReservationService;
        }

        public override EventType EventType => EventType.EndpointTransfer;
        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var endpointStatus = DeriveEndpointStatusForTransferEvent(callbackEvent);

            var command = new UpdateEndpointStatusAndRoomCommand(SourceConference.Id, SourceEndpoint.Id, endpointStatus,
                callbackEvent.TransferTo);
            await CommandHandler.Handle(command);
            
            if (endpointStatus == EndpointState.InConsultation && callbackEvent.TransferTo.HasValue)
            {
                _roomReservationService.RemoveRoomReservation(SourceConference.Id, callbackEvent.TransferTo.Value);
            }
        }
        
        private static EndpointState DeriveEndpointStatusForTransferEvent(CallbackEvent callbackEvent)
        {
            if (callbackEvent.TransferFrom == RoomType.WaitingRoom &&
                (callbackEvent.TransferTo == RoomType.ConsultationRoom1 ||
                 callbackEvent.TransferTo == RoomType.ConsultationRoom2))
                return EndpointState.InConsultation;

            if ((callbackEvent.TransferFrom == RoomType.ConsultationRoom1 ||
                 callbackEvent.TransferFrom == RoomType.ConsultationRoom2) &&
                callbackEvent.TransferTo == RoomType.WaitingRoom)
                return EndpointState.Connected;

            if ((callbackEvent.TransferFrom == RoomType.ConsultationRoom1 ||
                 callbackEvent.TransferFrom == RoomType.ConsultationRoom2) &&
                callbackEvent.TransferTo == RoomType.HearingRoom)
                return EndpointState.Connected;

            switch (callbackEvent.TransferFrom)
            {
                case RoomType.WaitingRoom when callbackEvent.TransferTo == RoomType.HearingRoom:
                    return EndpointState.Connected;
                case RoomType.HearingRoom when callbackEvent.TransferTo == RoomType.WaitingRoom:
                    return EndpointState.Connected;
                default:
                    throw new RoomTransferException(callbackEvent.TransferFrom.GetValueOrDefault(),
                        callbackEvent.TransferTo.GetValueOrDefault());
            }
        }
    }
}
