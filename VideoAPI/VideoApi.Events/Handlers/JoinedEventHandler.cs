using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Hub;
using VideoApi.Events.Models;
using VideoApi.Events.ServiceBus;

namespace VideoApi.Events.Handlers
{
    public class JoinedEventHandler : EventHandlerBase
    {
        public JoinedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IServiceBusQueueClient serviceBusQueueClient, IHubContext<EventHub, IEventHubClient> hubContext) : base(
            queryHandler, commandHandler, serviceBusQueueClient, hubContext)
        {
        }

        public override EventType EventType => EventType.Joined;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var isJudge = SourceParticipant.UserRole == UserRole.Judge;
            var participantState = isJudge ? ParticipantState.InHearing : ParticipantState.Available;
            var room = isJudge ? RoomType.HearingRoom : RoomType.WaitingRoom;

            await PublishParticipantStatusMessage(participantState).ConfigureAwait(false);
            if (isJudge) await PublishLiveEventMessage().ConfigureAwait(false);

            var command =
                new UpdateParticipantStatusAndRoomCommand(SourceConference.Id, SourceParticipant.Id, participantState,
                    room);
            await CommandHandler.Handle(command);
        }

        private async Task PublishLiveEventMessage()
        {
            var conferenceEvent = ConferenceState.InSession;
            await PublishConferenceStatusMessage(conferenceEvent);
            var command = new UpdateConferenceStatusCommand(SourceConference.Id, ConferenceState.InSession);
            await CommandHandler.Handle(command);
        }
    }
}