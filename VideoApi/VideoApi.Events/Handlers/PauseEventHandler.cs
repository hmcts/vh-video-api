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
    public class PauseEventHandler : EventHandlerBase<PauseEventHandler>
    {
        public PauseEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<PauseEventHandler> logger) : base(
            queryHandler, commandHandler, logger)
        {
        }

        public override EventType EventType => EventType.Pause;

        protected override Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var conferenceState = ConferenceState.Paused;
            var command = new UpdateConferenceStatusCommand(SourceConference.Id, conferenceState);
            
            _logger.LogInformation("{ConferenceId} Pause callback - {Tags}",
                SourceConference.Id, new [] {"VIH-7730", "HearingEvent"});
            return CommandHandler.Handle(command);
        }
    }
}
