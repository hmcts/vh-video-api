using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;

namespace VideoApi.Events.Handlers
{
    public class JoinedEventHandler : EventHandlerBase<JoinedEventHandler>
    {
        public JoinedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<JoinedEventHandler> logger) : base(
            queryHandler, commandHandler, logger)
        {
        }

        public override EventType EventType => EventType.Joined;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var participantState =  ParticipantState.Available;
            var room = RoomType.WaitingRoom;
            var participantIds = new List<Guid>
            {
                SourceParticipant.Id
            };
            participantIds.AddRange(SourceParticipant.LinkedParticipants
                .Where(p => p.Linked.State == ParticipantState.Available)
                .Select(linkedParticipant => linkedParticipant.LinkedId));
            
            foreach (var participantId in participantIds)
            {
                var command = new UpdateParticipantStatusAndRoomCommand(SourceConference.Id, participantId,
                    participantState, room, null);
            
                _logger.LogInformation("Joined callback - {ConferenceId}/{ParticipantId}",
                    SourceConference.Id, participantId);
                await CommandHandler.Handle(command);
            }
        }
    }
}
