using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;

namespace VideoApi.Events.Handlers
{
    public class RoomParticipantJoinedEventHandler : EventHandlerBase<RoomParticipantJoinedEventHandler>
    {
        public RoomParticipantJoinedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler,
            ILogger<RoomParticipantJoinedEventHandler> logger) : base(queryHandler, commandHandler, logger)
        {
        }

        public override EventType EventType => EventType.RoomParticipantJoined;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var participantState =  ParticipantState.Available;
            var room = RoomType.WaitingRoom;
            var updateParticipantCommand = new UpdateParticipantStatusAndRoomCommand(SourceConference.Id, SourceParticipant.Id,
                participantState, room, null);
            await CommandHandler.Handle(updateParticipantCommand);
            var addParticipantToRoomCommand =
                new AddParticipantToParticipantRoomCommand(SourceParticipantRoom.Id, SourceParticipant.Id);
            
            _logger.LogInformation("Room Participant Joined callback received - {ConferenceId}/{ParticipantId} - {ParticipantState} - {Room} {RoomLabel} - {SourceRoom}",
                SourceConference.Id, SourceParticipant.Id, participantState, room, null, SourceParticipantRoom.Id);

            await CommandHandler.Handle(addParticipantToRoomCommand);
        }
    }
}
