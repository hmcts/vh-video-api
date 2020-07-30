using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;

namespace VideoApi.Events.Handlers
{
    public class JoinedEventHandler : EventHandlerBase
    {
        public JoinedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler) : base(
            queryHandler, commandHandler)
        {
        }

        public override EventType EventType => EventType.Joined;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var participantState =  ParticipantState.Available;
            var room = RoomType.WaitingRoom;

            var command =
                new UpdateParticipantStatusAndRoomCommand(SourceConference.Id, SourceParticipant.Id, participantState,
                    room);
            await CommandHandler.Handle(command);
        }

       
    }
}
