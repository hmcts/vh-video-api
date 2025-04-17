using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using VideoApi.Common.Logging;

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

            Logger.LogRoomParticipantCallbackReceived(SourceConference.Id, SourceParticipant.Id, participantState.ToString(), room.ToString(), null, SourceParticipantRoom.Id);
            
            var participantIds = new List<Guid>
            {
                SourceParticipant.Id
            };

            participantIds.AddRange(SourceParticipant.LinkedParticipants
                .Where(p => p.Linked.State == ParticipantState.Available)
                .Select(linkedParticipant => linkedParticipant.LinkedId));

            foreach (var participantId in participantIds)
            {
                var updateParticipantCommand = new UpdateParticipantStatusAndRoomCommand(SourceConference.Id, participantId,
                    participantState, room, null);
                await CommandHandler.Handle(updateParticipantCommand);
                var addParticipantToRoomCommand =
                    new AddParticipantToParticipantRoomCommand(SourceParticipantRoom.Id, participantId);
            
                await CommandHandler.Handle(addParticipantToRoomCommand);
            }
        }
    }
}
