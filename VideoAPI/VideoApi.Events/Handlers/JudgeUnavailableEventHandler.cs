using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;


namespace VideoApi.Events.Handlers
{
    public class JudgeUnavailableEventHandler : EventHandlerBase
    {
        public JudgeUnavailableEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler) : base(
               queryHandler, commandHandler)
        {
        }

        public override EventType EventType => EventType.JudgeUnavailable;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var participantState = ParticipantState.NotSignedIn;

            var command = new UpdateParticipantStatusCommand(SourceConference.Id, SourceParticipant.Id, participantState);
            await CommandHandler.Handle(command);
        }
    }
}
