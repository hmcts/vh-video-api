using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;

namespace VideoApi.Events.Handlers
{
    public class SuspendEventHandler : EventHandlerBase
    {
        public SuspendEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler) : base(
            queryHandler, commandHandler)
        {
        }

        public override EventType EventType => EventType.Suspend;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var conferenceState = ConferenceState.Suspended;

            var command = new UpdateConferenceStatusCommand(SourceConference.Id, conferenceState);
            await CommandHandler.Handle(command);

            var reason = "Hearing suspended";
            if (SourceParticipant == null)
            {
                SourceParticipant = SourceConference.GetJudge();
                reason = "Technical assistance";
            }

            var taskCommand = new AddTaskCommand(SourceConference.Id, SourceParticipant.Id, reason, TaskType.Hearing);
            await CommandHandler.Handle(taskCommand);
        }
    }
}
