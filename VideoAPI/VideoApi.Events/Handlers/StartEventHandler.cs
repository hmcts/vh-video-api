using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;

namespace VideoApi.Events.Handlers
{
    public class StartEventHandler : EventHandlerBase
    {
        public StartEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler) : base(queryHandler, commandHandler)
        {
        }

        public override EventType EventType => EventType.Start;
        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var command = new UpdateConferenceStatusCommand(SourceConference.Id, ConferenceState.InSession);
            await CommandHandler.Handle(command);
        }
    }
}
