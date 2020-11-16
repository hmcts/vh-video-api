using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;

namespace VideoApi.Events.Handlers
{
    public class ConnectingToEventHubEventHandler : EventHandlerBase<ConnectingToEventHubEventHandler>
    {
        public ConnectingToEventHubEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<ConnectingToEventHubEventHandler> logger) : base(
            queryHandler, commandHandler, logger)
        {
        }

        public override EventType EventType => EventType.ConnectingToEventHub;

        protected override Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            return Task.CompletedTask;
        }
    }
}
