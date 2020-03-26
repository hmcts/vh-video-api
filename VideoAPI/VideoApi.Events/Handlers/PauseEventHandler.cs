using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;

namespace VideoApi.Events.Handlers
{
    public class PauseEventHandler : EventHandlerBase
    {
        public PauseEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler) : base(
            queryHandler, commandHandler)
        {
        }

        public override EventType EventType => EventType.Pause;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var conferenceState = ConferenceState.Paused;

            var command = new UpdateConferenceStatusCommand(SourceConference.Id, conferenceState);
            await CommandHandler.Handle(command);
        }
    }
}
