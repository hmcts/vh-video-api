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
            foreach (var participant in SourceConference.GetParticipants())
            {
                await HubContext.Clients.Group(participant.Username.ToLowerInvariant())
                    .HearingStatusMessage(SourceConference.HearingRefId, HearingStatus.Paused);
            }

            var hearingEventMessage = new HearingEventMessage
            {
                HearingId = SourceConference.HearingRefId,
                HearingStatus = HearingStatus.Paused,
            };

            await ServiceBusQueueClient.AddMessageToQueue(hearingEventMessage);
        }
    }
}