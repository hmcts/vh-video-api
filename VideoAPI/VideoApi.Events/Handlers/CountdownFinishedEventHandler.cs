using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;

namespace VideoApi.Events.Handlers
{
    public class CountdownFinishedEventHandler : EventHandlerBase<CountdownFinishedEventHandler>
    {
        public CountdownFinishedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<CountdownFinishedEventHandler> logger) : base(queryHandler, commandHandler, logger)
        {
        }

        public override EventType EventType => EventType.CountdownFinished;
        protected override Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            // simply a logging event
            return Task.CompletedTask;
        }
    }
}
