using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Events.Hub;
using VideoApi.Events.Models;
using VideoApi.Events.ServiceBus;

namespace VideoApi.Events.Handlers.Core
{
    public abstract class EventHandlerBase : IEventHandler
    {
        protected readonly IQueryHandler QueryHandler;
        protected readonly IServiceBusQueueClient ServiceBusQueueClient;
        protected readonly IHubContext<EventHub, IEventHubClient> HubContext;
        
        public Conference SourceConference { get; set; }
        public Participant SourceParticipant { get; set; }
        
        public abstract EventType EventType { get; }

        protected EventHandlerBase(IQueryHandler queryHandler, IServiceBusQueueClient serviceBusQueueClient,
            IHubContext<EventHub, IEventHubClient> hubContext)
        {
            QueryHandler = queryHandler;
            ServiceBusQueueClient = serviceBusQueueClient;
            HubContext = hubContext;
        }


        public async Task HandleAsync(CallbackEvent callbackEvent)
        {   
            if (!Guid.TryParse(callbackEvent.ConferenceId, out var conferenceId))
            {
                throw new ArgumentException("Invalid ConferenceId format");
            }

            SourceConference =
                await QueryHandler.Handle<GetConferenceByIdQuery, Conference>(new GetConferenceByIdQuery(conferenceId));

            if (SourceConference == null)
            {
                throw new ConferenceNotFoundException(conferenceId);
            }

            if (!long.TryParse(callbackEvent.ParticipantId, out var participantId))
            {
                throw new ArgumentException("Invalid ParticipantId format");
            }

            SourceParticipant = SourceConference.GetParticipants().SingleOrDefault(x => x.Id == participantId);
            
            await PublishStatusAsync(callbackEvent);
        }

        protected abstract Task PublishStatusAsync(CallbackEvent callbackEvent);
    }
}