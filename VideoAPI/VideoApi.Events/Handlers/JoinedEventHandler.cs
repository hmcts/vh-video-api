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
    public class JoinedEventHandler : EventHandlerBase
    {
        public JoinedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IServiceBusQueueClient serviceBusQueueClient, IHubContext<EventHub, IEventHubClient> hubContext) : base(
            queryHandler, commandHandler, serviceBusQueueClient, hubContext)
        {
        }

        public override EventType EventType => EventType.Joined;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var isJudge = SourceParticipant.UserRole == UserRole.Judge;
            var participantState = isJudge ? ParticipantState.InHearing : ParticipantState.Available;

            var command =
                new UpdateParticipantStatusCommand(SourceConference.Id, SourceParticipant.Id, participantState);
            await CommandHandler.Handle(command);
            await PublishParticipantStatusMessage(participantState);

            if (isJudge) await PublishLiveEventMessage();
        }

        private async Task PublishLiveEventMessage()
        {
            var conferenceEvent = ConferenceState.InSession;
            var hearingEventMessage = new HearingEventMessage
            {
                HearingRefId = SourceConference.HearingRefId,
                ConferenceStatus = conferenceEvent
            };

            await PublishConferenceStatusMessage(conferenceEvent);
            await ServiceBusQueueClient.AddMessageToQueue(hearingEventMessage);
        }
    }
}