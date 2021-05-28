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
    public class SuspendEventHandler : EventHandlerBase<SuspendEventHandler>
    {
        public SuspendEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<SuspendEventHandler> logger) : base(
            queryHandler, commandHandler, logger)
        {
        }

        public override EventType EventType => EventType.Suspend;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            const ConferenceState conferenceState = ConferenceState.Suspended;

            var command = new UpdateConferenceStatusCommand(SourceConference.Id, conferenceState);
            
            await CommandHandler.Handle(command);

            var reason = "Hearing suspended";
            
            if (SourceParticipant == null)
            {
                SourceParticipant = SourceConference.GetJudge();
                reason = "Technical assistance";
            }

            var taskCommand = new AddTaskCommand(SourceConference.Id, SourceParticipant.Id, reason, TaskType.Hearing);
            _logger.LogInformation("Suspend callback received - {ConferenceId} - {Tags}",
                SourceConference.Id, new [] {"VIH-7730", "HearingEvent"});
            await CommandHandler.Handle(taskCommand);
        }
    }
}
