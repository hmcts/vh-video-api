using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Hub;
using VideoApi.Events.Models;
using VideoApi.Events.ServiceBus;

namespace VideoApi.Events.Handlers
{
    public class DisconnectedEventHandler : EventHandlerBase
    {
        public DisconnectedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IServiceBusQueueClient serviceBusQueueClient, IHubContext<EventHub, IEventHubClient> hubContext) : base(
            queryHandler, commandHandler, serviceBusQueueClient, hubContext)
        {
        }

        public override EventType EventType => EventType.Disconnected;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            await PublishParticipantDisconnectMessage();

            await AddSuspendedTask();
            if (SourceParticipant.UserRole == UserRole.Judge) await PublishSuspendedEventMessage();
        }

        private async Task PublishParticipantDisconnectMessage()
        {
            var participantState = ParticipantState.Disconnected;
            var command =
                new UpdateParticipantStatusCommand(SourceConference.Id, SourceParticipant.Id, participantState);
            await CommandHandler.Handle(command);
            await PublishParticipantStatusMessage(participantState);
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
            await PublishConferenceStatusMessage(conferenceState);
            
            var updateConferenceStatusCommand =
                new UpdateConferenceStatusCommand(SourceConference.Id, conferenceState);
            await CommandHandler.Handle(updateConferenceStatusCommand);

            var hearingEventMessage = new HearingEventMessage
            {
                HearingRefId = SourceConference.HearingRefId,
                ConferenceStatus = conferenceState
            };
            await ServiceBusQueueClient.AddMessageToQueue(hearingEventMessage);
        }
    }
}