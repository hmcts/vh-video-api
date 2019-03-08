using System;
using System.Diagnostics.CodeAnalysis;
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
using VideoApi.Events.Models.Enums;
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

#pragma warning disable S4457 // Parameter validation in "async/await" methods should be wrapped
        public async Task HandleAsync(CallbackEvent callbackEvent)
        {
            SourceConference =
                await QueryHandler.Handle<GetConferenceByIdQuery, Conference>(
                    new GetConferenceByIdQuery(callbackEvent.ConferenceId));

            if (SourceConference == null)
            {
                throw new ConferenceNotFoundException(callbackEvent.ConferenceId);
            }

            SourceParticipant = SourceConference.GetParticipants()
                .SingleOrDefault(x => x.Id == callbackEvent.ParticipantId);

            await PublishStatusAsync(callbackEvent);
        }

        /// <summary>
        /// Publish a participant event to all participants in conference
        /// </summary>
        /// <param name="participantEventStatus">Participant status event to publish</param>
        /// <returns></returns>
        protected async Task PublishParticipantStatusMessage(ParticipantEventStatus participantEventStatus)
        {
            foreach (var participant in SourceConference.GetParticipants())
            {
                await HubContext.Clients.Group(participant.Username.ToLowerInvariant())
                    .ParticipantStatusMessage(SourceParticipant.Username, participantEventStatus);
            }
        }

        /// <summary>
        /// Publish a hearing event to all participants in conference
        /// </summary>
        /// <param name="hearingEventStatus">Hearing status event to publish</param>
        /// <returns></returns>
        protected async Task PublishHearingStatusMessage(HearingEventStatus hearingEventStatus)
        {
            foreach (var participant in SourceConference.GetParticipants())
            {
                await HubContext.Clients.Group(participant.Username.ToLowerInvariant())
                    .HearingStatusMessage(SourceConference.HearingRefId, hearingEventStatus);
            }
        }

        protected abstract Task PublishStatusAsync(CallbackEvent callbackEvent);
    }
}