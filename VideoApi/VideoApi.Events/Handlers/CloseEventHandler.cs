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
    public class CloseEventHandler : EventHandlerBase<CloseEventHandler>
    {
        public CloseEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<CloseEventHandler> logger) : base(
            queryHandler, commandHandler, logger)
        {
        }

        public override EventType EventType => EventType.Close;

        protected override Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var conferenceState = ConferenceState.Closed;

            var command = new UpdateConferenceStatusCommand(SourceConference.Id, conferenceState);
            
            _logger.LogInformation("{ConferenceId} Close callback - {Tags}",
                SourceConference.Id, new [] {"VIH-7730", "HearingEvent"});
            return CommandHandler.Handle(command);
        }
    }
}
