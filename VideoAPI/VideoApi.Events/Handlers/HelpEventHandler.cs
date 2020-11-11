using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;

namespace VideoApi.Events.Handlers
{
    public class HelpEventHandler : EventHandlerBase<HelpEventHandler>
    {
        public HelpEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<HelpEventHandler> logger) : base(
            queryHandler, commandHandler, logger)
        {
        }

        public override EventType EventType => EventType.Help;

        public override Task HandleAsync(CallbackEvent callbackEvent)
        {
            return PublishStatusAsync(callbackEvent);
        }

        protected override Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            return Task.CompletedTask;
        }

    }
}
