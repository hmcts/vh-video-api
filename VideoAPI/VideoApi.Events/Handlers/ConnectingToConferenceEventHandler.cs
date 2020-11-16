using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;

namespace VideoApi.Events.Handlers
{
    public class ConnectingToConferenceEventHandler : EventHandlerBase<ConnectingToConferenceEventHandler>
    {
        public ConnectingToConferenceEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<ConnectingToConferenceEventHandler> logger) : base(
            queryHandler, commandHandler, logger)
        {
        }

        public override EventType EventType => EventType.ConnectingToConference;

        protected override Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            return Task.CompletedTask;
        }
    }
}
