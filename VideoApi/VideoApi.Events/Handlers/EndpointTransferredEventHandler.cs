using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using Microsoft.Extensions.Logging;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Exceptions;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;

namespace VideoApi.Events.Handlers
{
    public class EndpointTransferredEventHandler : EventHandlerBase<EndpointTransferredEventHandler>
    {
        public EndpointTransferredEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<EndpointTransferredEventHandler> logger) : base(
            queryHandler, commandHandler, logger)
        {
        }

        public override EventType EventType => EventType.EndpointTransfer;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var endpointStatus = DeriveEndpointStatusForTransferEvent(callbackEvent);

            var command = new UpdateEndpointStatusAndRoomCommand(SourceConference.Id, SourceEndpoint.Id, endpointStatus,
                callbackEvent.TransferTo, callbackEvent.TransferredToRoomLabel);
            await CommandHandler.Handle(command);
        }
        
        private static EndpointState DeriveEndpointStatusForTransferEvent(CallbackEvent callbackEvent)
        {
            var toConsultationRoom = callbackEvent.TransferTo == RoomType.ConsultationRoom;
            
            if (callbackEvent.TransferFrom == RoomType.WaitingRoom && toConsultationRoom)
                return EndpointState.InConsultation;

            var fromConsultationRoom = callbackEvent.TransferFrom == RoomType.ConsultationRoom;
            if (fromConsultationRoom && callbackEvent.TransferTo == RoomType.WaitingRoom)
                return EndpointState.Connected;

            if (callbackEvent.TransferFrom == RoomType.ConsultationRoom &&
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
