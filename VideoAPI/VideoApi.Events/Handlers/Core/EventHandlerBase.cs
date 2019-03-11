using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using VideoApi.DAL.Commands.Core;
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
        protected readonly ICommandHandler CommandHandler;
        protected readonly IServiceBusQueueClient ServiceBusQueueClient;
        protected readonly IHubContext<EventHub, IEventHubClient> HubContext;

        public Conference SourceConference { get; set; }
        public Participant SourceParticipant { get; set; }

        public abstract EventType EventType { get; }

        protected EventHandlerBase(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IServiceBusQueueClient serviceBusQueueClient, IHubContext<EventHub, IEventHubClient> hubContext)
        {
            QueryHandler = queryHandler;
            CommandHandler = commandHandler;
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
        /// Publish a participant event to all participants in conference to those connected to the HubContext
        /// </summary>
        /// <param name="participantState">Participant status event to publish</param>
        /// <returns></returns>
        protected async Task PublishParticipantStatusMessage(ParticipantState participantState)
        {
            foreach (var participant in SourceConference.GetParticipants())
            {
                await HubContext.Clients.Group(participant.Username.ToLowerInvariant())
                    .ParticipantStatusMessage(SourceParticipant.Username, participantState);
            }
        }

        /// <summary>
        /// Publish a hearing event to all participants in conference to those connected to the HubContext
        /// </summary>
        /// <param name="hearingEventStatus">Hearing status event to publish</param>
        /// <returns></returns>
        protected async Task PublishConferenceStatusMessage(ConferenceState hearingEventStatus)
        {
            foreach (var participant in SourceConference.GetParticipants())
            {
                await HubContext.Clients.Group(participant.Username.ToLowerInvariant())
                    .ConferenceStatusMessage(SourceConference.HearingRefId, hearingEventStatus);
            }
        }

        protected abstract Task PublishStatusAsync(CallbackEvent callbackEvent);
    }
}