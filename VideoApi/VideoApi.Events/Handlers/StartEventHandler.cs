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
    public class StartEventHandler : EventHandlerBase<StartEventHandler>
    {
        public StartEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<StartEventHandler> logger) : base(
            queryHandler, commandHandler, logger)
        {
        }

        public override EventType EventType => EventType.Start;

        protected override Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var command = new UpdateConferenceStatusCommand(SourceConference.Id, ConferenceState.InSession);
            
            _logger.LogInformation("{ConferenceId} Start callback - {Tags}",
                SourceConference.Id, new [] {"VIH-7730", "HearingEvent"});
            return CommandHandler.Handle(command);
        }
    }
}
