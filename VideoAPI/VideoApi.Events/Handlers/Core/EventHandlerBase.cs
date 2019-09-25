using System.Linq;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Events.Models;
using VideoApi.Events.ServiceBus;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Events.Handlers.Core
{
    public abstract class EventHandlerBase : IEventHandler
    {
        protected readonly ICommandHandler CommandHandler;
        protected readonly IQueryHandler QueryHandler;
        protected readonly IServiceBusQueueClient ServiceBusQueueClient;

        protected EventHandlerBase(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IServiceBusQueueClient serviceBusQueueClient)
        {
            QueryHandler = queryHandler;
            CommandHandler = commandHandler;
            ServiceBusQueueClient = serviceBusQueueClient;
        }

        public Conference SourceConference { get; set; }
        public Participant SourceParticipant { get; set; }

        public abstract EventType EventType { get; }
#pragma warning disable S4457 // Parameter validation in "async/await" methods should be wrapped
        public async Task HandleAsync(CallbackEvent callbackEvent)
        {
            SourceConference =
                await QueryHandler.Handle<GetConferenceByIdQuery, Conference>(
                    new GetConferenceByIdQuery(callbackEvent.ConferenceId));

            if (SourceConference == null) throw new ConferenceNotFoundException(callbackEvent.ConferenceId);

            SourceParticipant = SourceConference.GetParticipants()
                .SingleOrDefault(x => x.Id == callbackEvent.ParticipantId);

            await PublishStatusAsync(callbackEvent);
        }

        protected abstract Task PublishStatusAsync(CallbackEvent callbackEvent);
    }
}