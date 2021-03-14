using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Linq;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Events.Models;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Events.Handlers.Core
{
    public abstract class EventHandlerBase<THandler> : IEventHandler
    {
        protected readonly ICommandHandler CommandHandler;
        protected readonly IQueryHandler QueryHandler;
        protected readonly ILogger<THandler> _logger;

        protected EventHandlerBase(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<THandler> logger)
        {
            QueryHandler = queryHandler;
            CommandHandler = commandHandler;
            _logger = logger;
        }

        protected Conference SourceConference { get; set; }
        protected Participant SourceParticipant { get; set; }
        protected Endpoint SourceEndpoint { get; set; }
        protected InterpreterRoom SourceInterpreterRoom { get; set; }

        public abstract EventType EventType { get; }

        public virtual async Task HandleAsync(CallbackEvent callbackEvent)
        {
            _logger.LogDebug("Handling callback");
            var sw = Stopwatch.StartNew();
            SourceConference =
                await QueryHandler.Handle<GetConferenceByIdQuery, Conference>(
                    new GetConferenceByIdQuery(callbackEvent.ConferenceId));

            if (SourceConference == null) throw new ConferenceNotFoundException(callbackEvent.ConferenceId);

            SourceParticipant = SourceConference.GetParticipants()
                .SingleOrDefault(x => x.Id == callbackEvent.ParticipantId);

            SourceEndpoint = SourceConference.GetEndpoints().SingleOrDefault(x => x.Id == callbackEvent.ParticipantId);
            SourceInterpreterRoom = SourceConference.Rooms.Cast<InterpreterRoom>().SingleOrDefault(x => x.Id == callbackEvent.RoomId);
            
            await PublishStatusAsync(callbackEvent);
            _logger.LogDebug("Handled callback in {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);
        }

        protected abstract Task PublishStatusAsync(CallbackEvent callbackEvent);
    }
}
