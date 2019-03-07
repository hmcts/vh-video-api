using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Exceptions;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Hub;
using VideoApi.Events.Models;
using VideoApi.Events.Models.Enums;
using VideoApi.Events.ServiceBus;

namespace VideoApi.Events.Handlers
{
    public class TransferEventHandler : EventHandlerBase
    {
        public TransferEventHandler(IQueryHandler queryHandler, IServiceBusQueueClient serviceBusQueueClient,
            IHubContext<EventHub, IEventHubClient> hubContext) : base(queryHandler, serviceBusQueueClient, hubContext)
        {
        }

        public override EventType EventType => EventType.Transfer;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var participantStatus = DeriveParticipantStatusForTransferEvent(callbackEvent);

            var participantEventMessage = new ParticipantEventMessage
            {
                HearingId = SourceConference.HearingRefId,
                ParticipantId = SourceParticipant.ParticipantRefId,
                ParticipantEventStatus = participantStatus
            };

            await PublishParticipantStatusMessage(participantStatus);
            await ServiceBusQueueClient.AddMessageToQueue(participantEventMessage);
        }
        
        private static ParticipantEventStatus DeriveParticipantStatusForTransferEvent(CallbackEvent callbackEvent)
        {
            if (callbackEvent.TransferFrom == RoomType.WaitingRoom &&
                (callbackEvent.TransferTo == RoomType.ConsultationRoom1 ||
                 callbackEvent.TransferTo == RoomType.ConsultationRoom2))
            {
                return ParticipantEventStatus.InConsultation;
            }

            if ((callbackEvent.TransferFrom == RoomType.ConsultationRoom1 ||
                 callbackEvent.TransferFrom == RoomType.ConsultationRoom2) &&
                callbackEvent.TransferTo == RoomType.WaitingRoom)
            {
                return ParticipantEventStatus.Available;
            }

            switch(callbackEvent.TransferFrom)
            {
                case RoomType.WaitingRoom when callbackEvent.TransferTo == RoomType.HearingRoom:
                    return ParticipantEventStatus.InHearing;
                case RoomType.HearingRoom when callbackEvent.TransferTo == RoomType.WaitingRoom:
                    return ParticipantEventStatus.Available; // this needs to be InWaitingRoom but we do not have this status on hearing api 
                default:
                    throw new RoomTransferException(callbackEvent.TransferFrom.GetValueOrDefault(), callbackEvent.TransferTo.GetValueOrDefault());
            }

            
        }
    }
}