using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Hub;
using VideoApi.Events.ServiceBus;

namespace VideoApi.Events.Handlers
{
    public abstract class EventHandlerBase : IEventHandler
    {
        protected readonly IQueryHandler QueryHandler;
        protected readonly IServiceBusQueueClient ServiceBusQueueClient;
        protected readonly IHubContext<EventHub, IEventHubClient> HubContext;
        
        public abstract EventType EventType { get; }

        protected EventHandlerBase(IQueryHandler queryHandler, IServiceBusQueueClient serviceBusQueueClient,
            IHubContext<EventHub, IEventHubClient> hubContext)
        {
            QueryHandler = queryHandler;
            ServiceBusQueueClient = serviceBusQueueClient;
            HubContext = hubContext;
        }


        public Task HandleAsync(ConferenceEventRequest conferenceEventRequest)
        {
            throw new System.NotImplementedException();
        }
        
        protected abstract Task PublishStatusAsync(ConferenceEventRequest conferenceEventRequest);
    }
}