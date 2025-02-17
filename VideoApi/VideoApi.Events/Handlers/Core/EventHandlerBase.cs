using System;
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
using VideoApi.Events.Exceptions;
using VideoApi.Events.Models;
using Task = System.Threading.Tasks.Task;

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
            Logger.LogDebug("Handling callback");
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
            Logger.LogDebug("Handled callback in {ElapsedMilliseconds}ms", sw.ElapsedMilliseconds);
        }

        protected abstract Task PublishStatusAsync(CallbackEvent callbackEvent);

        protected void ValidateParticipantEventReceivedAfterLastUpdate(CallbackEvent callbackEvent)
        {
            var eventReceivedAfterLastUpdate = SourceParticipant.UpdatedAt > callbackEvent.TimeStampUtc;
            if (!eventReceivedAfterLastUpdate) return;
            var innerException = new InvalidOperationException(
                $"Participant {SourceParticipant.Id} has already been updated since this event {callbackEvent.EventType} with the time {callbackEvent.TimeStampUtc}. Current Status: {SourceParticipant.GetCurrentStatus().ParticipantState} - Last Updated At: {SourceParticipant.UpdatedAt}");
            Logger.LogError(new UnexpectedEventOrderException(callbackEvent, innerException), "Unexpected event order for participant");
        }

        protected void ValidateJvsEventReceivedAfterLastUpdate(CallbackEvent callbackEvent)
        {
            var eventReceivedAfterLastUpdate = SourceEndpoint.UpdatedAt > callbackEvent.TimeStampUtc;
            if(!eventReceivedAfterLastUpdate) return;
            var innerException = new InvalidOperationException(
                $"Endpoint {SourceEndpoint.Id} has already been updated since this event {callbackEvent.EventType} with the time {callbackEvent.TimeStampUtc}. Current Status: {SourceEndpoint.State} - Last Updated At: {SourceEndpoint.UpdatedAt}");
            Logger.LogError(new UnexpectedEventOrderException(callbackEvent, innerException), "Unexpected event order for endpoint");
        }
        
        protected void ValidateTelephoneParticipantEventReceivedAfterLastUpdate(CallbackEvent callbackEvent)
        {
            // Telephone participants are added dynamically so we have to use the null operator
            var eventReceivedAfterLastUpdate = SourceTelephoneParticipant?.UpdatedAt > callbackEvent.TimeStampUtc;
            if(!eventReceivedAfterLastUpdate) return;
            var innerException = new InvalidOperationException(
                $"TelephoneParticipant {SourceTelephoneParticipant.Id} has already been updated since this event {callbackEvent.EventType} with the time {callbackEvent.TimeStampUtc}. Current Status: {SourceTelephoneParticipant.State} - Last Updated At: {SourceTelephoneParticipant.UpdatedAt}");
            Logger.LogError(new UnexpectedEventOrderException(callbackEvent, innerException), "Unexpected event order for telephone participant");
        }
    }
}
