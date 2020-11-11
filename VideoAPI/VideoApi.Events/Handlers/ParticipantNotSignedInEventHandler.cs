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
    public class ParticipantNotSignedInEventHandler : EventHandlerBase<ParticipantNotSignedInEventHandler>
    {
        public ParticipantNotSignedInEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<ParticipantNotSignedInEventHandler> logger) : base(
            queryHandler, commandHandler, logger)
        {
        }

        public override EventType EventType => EventType.ParticipantNotSignedIn;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var participantState = ParticipantState.NotSignedIn;

            var command = new UpdateParticipantStatusCommand(SourceConference.Id,
                SourceParticipant.Id, participantState);
            await CommandHandler.Handle(command);
        }
    }
}
