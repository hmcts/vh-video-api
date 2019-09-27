using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using VideoApi.Events.ServiceBus;

namespace VideoApi.Events.Handlers
{
    public class DisconnectedEventHandler : EventHandlerBase
    {
        public DisconnectedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IServiceBusQueueClient serviceBusQueueClient) : base(
            queryHandler, commandHandler, serviceBusQueueClient)
        {
        }

        public override EventType EventType => EventType.Disconnected;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            await PublishParticipantDisconnectMessage().ConfigureAwait(false);
            if (SourceParticipant.IsJudge()) await PublishSuspendedEventMessage().ConfigureAwait(false);
        }

        private async Task PublishParticipantDisconnectMessage()
        {
            var participantState = ParticipantState.Disconnected;
            var command =
                new UpdateParticipantStatusAndRoomCommand(SourceConference.Id, SourceParticipant.Id, participantState,
                    null);
            await CommandHandler.Handle(command).ConfigureAwait(false);
            await AddDisconnectedTask().ConfigureAwait(false);
        }

        private async Task AddDisconnectedTask()
        {
            var taskType = SourceParticipant.IsJudge() ? TaskType.Judge : TaskType.Participant;
            var disconnected = new AddTaskCommand(SourceConference.Id, SourceParticipant.Id, "Disconnected", taskType);
            await CommandHandler.Handle(disconnected).ConfigureAwait(false);
        }

        private async Task AddSuspendedTask()
        {
            var addSuspendedTask =
                new AddTaskCommand(SourceConference.Id, SourceConference.Id, "Suspended", TaskType.Hearing);
            await CommandHandler.Handle(addSuspendedTask).ConfigureAwait(false);
        }

        private async Task PublishSuspendedEventMessage()
        {
            var conferenceState = ConferenceState.Suspended;
            var updateConferenceStatusCommand =
                new UpdateConferenceStatusCommand(SourceConference.Id, conferenceState);
            await CommandHandler.Handle(updateConferenceStatusCommand).ConfigureAwait(false);

            await AddSuspendedTask().ConfigureAwait(false);
        }
    }
}