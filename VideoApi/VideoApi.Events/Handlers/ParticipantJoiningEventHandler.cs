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
    public class ParticipantJoiningEventHandler : EventHandlerBase<ParticipantJoiningEventHandler>
    {
        public ParticipantJoiningEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<ParticipantJoiningEventHandler> logger) : base(
            queryHandler, commandHandler, logger)
        {
        }

        public override EventType EventType => EventType.ParticipantJoining;

        protected override Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var participantState = ParticipantState.Joining;
            var command = new UpdateParticipantStatusCommand(SourceConference.Id, SourceParticipant.Id, participantState);
            return CommandHandler.Handle(command);
        }
    }
}
