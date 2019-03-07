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
    public class CloseEventHandler : EventHandlerBase
    {
        public CloseEventHandler(IQueryHandler queryHandler, IServiceBusQueueClient serviceBusQueueClient,
            IHubContext<EventHub, IEventHubClient> hubContext) : base(queryHandler, serviceBusQueueClient, hubContext)
        {
        }

        public override EventType EventType => EventType.Close;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            await PublishHearingStatusMessage(HearingEventStatus.Closed);
            
            var hearingEventMessage = new HearingEventMessage
            {
                HearingId = SourceConference.HearingRefId,
                HearingEventStatus = HearingEventStatus.Closed
            };

            await ServiceBusQueueClient.AddMessageToQueue(hearingEventMessage);
        }
    }
}