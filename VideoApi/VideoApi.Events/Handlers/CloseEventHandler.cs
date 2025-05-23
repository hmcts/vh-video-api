using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using VideoApi.Common.Logging;

namespace VideoApi.Events.Handlers
{
    public class CloseEventHandler(
        IQueryHandler queryHandler,
        ICommandHandler commandHandler,
        ILogger<CloseEventHandler> logger)
        : EventHandlerBase<CloseEventHandler>(queryHandler, commandHandler, logger)
    {
        public override EventType EventType => EventType.Close;

        private static readonly string[] Args = ["VIH-7730", "HearingEvent"];

        protected override Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var conferenceState = ConferenceState.Closed;

            var command = new UpdateConferenceStatusCommand(SourceConference.Id, conferenceState);
            
            Logger.LogCloseCallback(SourceConference.Id, string.Join(",", Args));
            return CommandHandler.Handle(command);
        }
    }
}
