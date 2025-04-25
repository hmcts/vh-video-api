using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Events.Models;
using Task = System.Threading.Tasks.Task;
using VideoApi.Common.Logging;

namespace VideoApi.Events.Handlers.Core
{
    [SuppressMessage("csharpsquid", "S6672:Generic logger injection should match enclosing type")]
    public abstract class EventHandlerBase<THandler>(
        IQueryHandler queryHandler,
        ICommandHandler commandHandler,
        ILogger<THandler> logger)
        : IEventHandler
    {
        protected readonly ICommandHandler CommandHandler = commandHandler;
        protected readonly IQueryHandler QueryHandler = queryHandler;
        protected readonly ILogger<THandler> Logger = logger;

        protected Conference SourceConference { get; set; }
        protected ParticipantBase SourceParticipant { get; set; }
        protected Endpoint SourceEndpoint { get; set; }
        protected TelephoneParticipant SourceTelephoneParticipant { get; set; }
        protected ParticipantRoom SourceParticipantRoom { get; set; }

        public abstract EventType EventType { get; }

        public virtual async Task HandleAsync(CallbackEvent callbackEvent)
        {
            Logger.LogHandlingCallback();
            var sw = Stopwatch.StartNew();
            SourceConference =
                await QueryHandler.Handle<GetConferenceByIdForEventQuery, Conference>(
                    new GetConferenceByIdForEventQuery(callbackEvent.ConferenceId));

            if (SourceConference == null) throw new ConferenceNotFoundException(callbackEvent.ConferenceId);

            SourceParticipant = SourceConference.GetParticipants()
                .SingleOrDefault(x => x.Id == callbackEvent.ParticipantId);

            SourceEndpoint = SourceConference.GetEndpoints().SingleOrDefault(x => x.Id == callbackEvent.ParticipantId);
            SourceTelephoneParticipant = SourceConference.GetTelephoneParticipants().SingleOrDefault(x => x.Id == callbackEvent.ParticipantId);
            SourceParticipantRoom = SourceConference.Rooms.OfType<ParticipantRoom>().SingleOrDefault(x =>x.Id == callbackEvent.ParticipantRoomId);
            
            await PublishStatusAsync(callbackEvent);
            Logger.LogHandlingTimeCallback(sw.ElapsedMilliseconds);
        }

        protected abstract Task PublishStatusAsync(CallbackEvent callbackEvent);
    }
}
