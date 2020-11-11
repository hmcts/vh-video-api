using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;

namespace VideoApi.Events.Handlers
{
    public class EndpointDisconnectedEventHandler : EventHandlerBase<EndpointDisconnectedEventHandler>
    {
        public EndpointDisconnectedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<EndpointDisconnectedEventHandler> logger) : base(
            queryHandler, commandHandler, logger)
        {
        }

        public override EventType EventType => EventType.EndpointDisconnected;
        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            const EndpointState endpointState = EndpointState.Disconnected;
            var command =
                new UpdateEndpointStatusAndRoomCommand(SourceConference.Id, SourceEndpoint.Id, endpointState, null);
            await CommandHandler.Handle(command);
        }
    }
}
