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
    public class DisconnectedEventHandler : EventHandlerBase
    {
        public DisconnectedEventHandler(IQueryHandler queryHandler, IServiceBusQueueClient serviceBusQueueClient,
            IHubContext<EventHub, IEventHubClient> hubContext) : base(queryHandler, serviceBusQueueClient, hubContext)
        {
        }

        public override EventType EventType => EventType.Disconnected;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            await PublishParticipantDisconnectMessage();

            if (SourceParticipant.UserRole == UserRole.Judge)
            {
                await PublishSuspendedEventMessage();
            }
        }

        private async Task PublishParticipantDisconnectMessage()
        {
            await PublishParticipantStatusMessage(ParticipantEventStatus.Disconnected);
            
            var participantEventMessage = new ParticipantEventMessage
            {
                HearingId = SourceConference.HearingRefId,
                ParticipantId = SourceParticipant.ParticipantRefId,
                ParticipantEventStatus = ParticipantEventStatus.Disconnected
            };
            await ServiceBusQueueClient.AddMessageToQueue(participantEventMessage);
        }

        private async Task PublishSuspendedEventMessage()
        {
            var hearingEventMessage = new HearingEventMessage
            {
                HearingId = SourceConference.HearingRefId,
                HearingEventStatus = HearingEventStatus.Suspended
            };

            await PublishHearingStatusMessage(HearingEventStatus.Suspended);
            await ServiceBusQueueClient.AddMessageToQueue(hearingEventMessage);
        }
    }
}