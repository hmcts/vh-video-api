using Microsoft.Extensions.Logging;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Events.Handlers
{
    public class DisconnectedEventHandler : EventHandlerBase<DisconnectedEventHandler>
    {
        public DisconnectedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<DisconnectedEventHandler> logger) : base(
            queryHandler, commandHandler, logger)
        {
        }

        public override EventType EventType => EventType.Disconnected;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            Logger.LogInformation("Disconnected callback - {ConferenceId}/{ParticipantId}",
                SourceConference.Id, SourceParticipant.Id);
            await PublishParticipantDisconnectMessage();
        }

        private async Task PublishParticipantDisconnectMessage()
        {
            var participantState = ParticipantState.Disconnected;
            var command =
                new UpdateParticipantStatusAndRoomCommand(SourceConference.Id, SourceParticipant.Id, participantState,
                    null, null);
            await CommandHandler.Handle(command);
            if (SourceConference.State != ConferenceState.Closed)
            {
                await AddDisconnectedTask();
            }
        }

        private async Task AddDisconnectedTask()
        {
            var taskType = SourceParticipant is Participant && ((Participant)SourceParticipant).IsJudge() ? TaskType.Judge : TaskType.Participant;
            var disconnected = new AddTaskCommand(SourceConference.Id, SourceParticipant.Id, "Disconnected", taskType);

            await CommandHandler.Handle(disconnected);
        }
    }
}
