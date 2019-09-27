using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using VideoApi.Events.ServiceBus;

namespace VideoApi.Events.Handlers
{
    public class JoinedEventHandler : EventHandlerBase
    {
        public JoinedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IServiceBusQueueClient serviceBusQueueClient) : base(
            queryHandler, commandHandler, serviceBusQueueClient)
        {
        }

        public override EventType EventType => EventType.Joined;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var participantState = SourceParticipant.IsJudge() ? ParticipantState.InHearing : ParticipantState.Available;
            var room = SourceParticipant.IsJudge() ? RoomType.HearingRoom : RoomType.WaitingRoom;

            if (SourceParticipant.IsJudge()) await PublishLiveEventMessage();

            var command =
                new UpdateParticipantStatusAndRoomCommand(SourceConference.Id, SourceParticipant.Id, participantState,
                    room);
            await CommandHandler.Handle(command);
        }

        private async Task PublishLiveEventMessage()
        {
            var conferenceEvent = ConferenceState.InSession;
            var command = new UpdateConferenceStatusCommand(SourceConference.Id, ConferenceState.InSession);
            await CommandHandler.Handle(command);
        }
    }
}