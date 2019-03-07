using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Hub;
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

        protected override Task PublishStatusAsync(ConferenceEventRequest conferenceEventRequest)
        {
            throw new System.NotImplementedException();
        }
    }
}