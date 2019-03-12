using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Hub;
using VideoApi.Events.Models;
using VideoApi.Events.ServiceBus;

namespace VideoApi.Events.Handlers
{
    public class PauseEventHandler : EventHandlerBase
    {
        public PauseEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IServiceBusQueueClient serviceBusQueueClient, IHubContext<EventHub, IEventHubClient> hubContext) : base(
            queryHandler, commandHandler, serviceBusQueueClient, hubContext)
        {
        }

        public override EventType EventType => EventType.Pause;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var hearingState = ConferenceState.Paused;
            await PublishConferenceStatusMessage(hearingState);
            var hearingEventMessage = new HearingEventMessage
            {
                HearingRefId = SourceConference.HearingRefId,
                ConferenceStatus = hearingState
            };

            await ServiceBusQueueClient.AddMessageToQueue(hearingEventMessage);
        }
    }
}