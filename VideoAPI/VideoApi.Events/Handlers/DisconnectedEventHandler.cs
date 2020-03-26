using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;

namespace VideoApi.Events.Handlers
{
    public class DisconnectedEventHandler : EventHandlerBase
    {
        public DisconnectedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler) : base(
            queryHandler, commandHandler)
        {
        }

        public override EventType EventType => EventType.Disconnected;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            await PublishParticipantDisconnectMessage();
            if (SourceParticipant.IsJudge()) await PublishSuspendedEventMessage();
        }

        private async Task PublishParticipantDisconnectMessage()
        {
            var participantState = ParticipantState.Disconnected;
            var command =
                new UpdateParticipantStatusAndRoomCommand(SourceConference.Id, SourceParticipant.Id, participantState,
                    null);
            await CommandHandler.Handle(command);
            if (SourceConference.State != ConferenceState.Closed)
            {
                await AddDisconnectedTask();
            }            
        }

        private async Task AddDisconnectedTask()
        {
            var taskType = SourceParticipant.IsJudge() ? TaskType.Judge : TaskType.Participant;
            var disconnected = new AddTaskCommand(SourceConference.Id, SourceParticipant.Id, "Disconnected", taskType);
            await CommandHandler.Handle(disconnected);
        }

        private async Task AddSuspendedTask()
        {
            var addSuspendedTask =
                new AddTaskCommand(SourceConference.Id, SourceConference.Id, "Suspended", TaskType.Hearing);
            await CommandHandler.Handle(addSuspendedTask);
        }

        private async Task PublishSuspendedEventMessage()
        {
            var conferenceState = ConferenceState.Suspended;
            var updateConferenceStatusCommand =
                new UpdateConferenceStatusCommand(SourceConference.Id, conferenceState);
            await CommandHandler.Handle(updateConferenceStatusCommand);

            await AddSuspendedTask();
        }
    }
}
