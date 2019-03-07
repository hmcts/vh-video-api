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
    public class JudgeAvailableEventHandler : EventHandlerBase
    {
        public JudgeAvailableEventHandler(IQueryHandler queryHandler, IServiceBusQueueClient serviceBusQueueClient,
            IHubContext<EventHub, IEventHubClient> hubContext) : base(queryHandler, serviceBusQueueClient, hubContext)
        {
        }

        public override EventType EventType => EventType.JudgeAvailable;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            await PublishParticipantStatusMessage(ParticipantEventStatus.Available);
            
            var participantEventMessage = new ParticipantEventMessage
            {
                HearingId = SourceConference.HearingRefId,
                ParticipantId = SourceParticipant.ParticipantRefId,
                ParticipantEventStatus = ParticipantEventStatus.Available
            };

            await ServiceBusQueueClient.AddMessageToQueue(participantEventMessage);
        }
    }
}