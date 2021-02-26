using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using Microsoft.Extensions.Logging;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
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
            if ((!callbackEvent.TransferTo.HasValue || callbackEvent.TransferTo == RoomType.ConsultationRoom) && callbackEvent.TransferredToRoomLabel.ToLower().Contains("consultation"))
            {
                return EndpointState.InConsultation;
            }

            return EndpointState.Connected;
        }
    }
}
