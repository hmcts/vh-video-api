using System;
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
    public class VhOfficerCallEventHandler : EventHandlerBase
    {
        public VhOfficerCallEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IServiceBusQueueClient serviceBusQueueClient, IHubContext<EventHub, IEventHubClient> hubContext) : base(
            queryHandler, commandHandler, serviceBusQueueClient, hubContext)
        {
        }

        public override EventType EventType => EventType.VhoCall;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var targetRoom = ValidationConsultationRoom(callbackEvent);
            await HubContext.Clients.Group(SourceParticipant.Username.ToLowerInvariant())
                .AdminConsultationMessage(SourceConference.Id, targetRoom,
                    SourceParticipant.Username.ToLowerInvariant());
        }

        private RoomType ValidationConsultationRoom(CallbackEvent callbackEvent)
        {
            if (!callbackEvent.TransferTo.HasValue || callbackEvent.TransferTo.Value != RoomType.ConsultationRoom1 
                && callbackEvent.TransferTo.Value != RoomType.ConsultationRoom2) 
            {
                throw new ArgumentException("No consultation room provided");
            }

            return callbackEvent.TransferTo.Value;
        }
    }
}