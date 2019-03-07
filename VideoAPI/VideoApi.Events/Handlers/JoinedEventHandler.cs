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
    public class JoinedEventHandler : EventHandlerBase
    {
        public JoinedEventHandler(IQueryHandler queryHandler, IServiceBusQueueClient serviceBusQueueClient,
            IHubContext<EventHub, IEventHubClient> hubContext) : base(queryHandler, serviceBusQueueClient, hubContext)
        {
        }

        public override EventType EventType => EventType.Joined;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var isJudge = SourceParticipant.UserRole == UserRole.Judge;
            var participantStatus = isJudge ? ParticipantEventStatus.InHearing : ParticipantEventStatus.Available;

            await PublishParticipantStatusMessage(participantStatus);
            
            var participantEventMessage = new ParticipantEventMessage
            {
                HearingId = SourceConference.HearingRefId,
                ParticipantId = SourceParticipant.ParticipantRefId,
                ParticipantEventStatus = participantStatus
            };
            await ServiceBusQueueClient.AddMessageToQueue(participantEventMessage);

            if (isJudge)
            {
                await PublishLiveEventMessage();
            }
        }
        
        private async Task PublishLiveEventMessage()
        {
            var hearingEventMessage = new HearingEventMessage
            {
                HearingId = SourceConference.HearingRefId,
                HearingEventStatus = HearingEventStatus.Live
            };

            await PublishHearingStatusMessage(HearingEventStatus.Live);
            await ServiceBusQueueClient.AddMessageToQueue(hearingEventMessage);
        }
    }
}