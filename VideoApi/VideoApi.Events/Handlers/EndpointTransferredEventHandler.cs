using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using Microsoft.Extensions.Logging;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using VideoApi.Common.Logging;

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
            Logger.LogEndpointTransferredCallback(SourceConference.Id, SourceEndpoint.Id);
            var endpointStatus = DeriveEndpointStatusForTransferEvent(callbackEvent);

            var command = new UpdateEndpointStatusAndRoomCommand(SourceConference.Id, SourceEndpoint.Id, endpointStatus,
                callbackEvent.TransferTo, callbackEvent.TransferredToRoomLabel);
            await CommandHandler.Handle(command);
        }
        
        private static EndpointState DeriveEndpointStatusForTransferEvent(CallbackEvent callbackEvent)
        {
            if (callbackEvent.TransferTo is null or RoomType.ConsultationRoom 
                && callbackEvent.TransferredToRoomLabel.Contains("consultation", System.StringComparison.CurrentCultureIgnoreCase))
            {
                return EndpointState.InConsultation;
            }
            
            if(callbackEvent.TransferTo == RoomType.HearingRoom)
            {
                return EndpointState.InHearing;
            }

            return EndpointState.Connected;
        }
    }
}
