using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Hub;
using VideoApi.Events.Models;
using VideoApi.Events.Models.Enums;
using VideoApi.Events.ServiceBus;

namespace VideoApi.Events.Handlers
{
    public class PauseEventHandler : EventHandlerBase
    {
        public PauseEventHandler(IQueryHandler queryHandler, IServiceBusQueueClient serviceBusQueueClient,
            IHubContext<EventHub, IEventHubClient> hubContext) : base(queryHandler, serviceBusQueueClient, hubContext)
        {
        }

        public override EventType EventType => EventType.Pause;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            await PublishHearingStatusMessage(HearingEventStatus.Paused);
            var hearingEventMessage = new HearingEventMessage
            {
                HearingId = SourceConference.HearingRefId,
                HearingEventStatus = HearingEventStatus.Paused,
            };

            await ServiceBusQueueClient.AddMessageToQueue(hearingEventMessage);
        }
    }
}