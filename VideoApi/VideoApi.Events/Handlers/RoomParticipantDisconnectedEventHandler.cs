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
    public class RoomParticipantDisconnectedEventHandler : EventHandlerBase<RoomParticipantDisconnectedEventHandler>
    {
        public RoomParticipantDisconnectedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler,
            ILogger<RoomParticipantDisconnectedEventHandler> logger) : base(queryHandler, commandHandler, logger)
        {
        }

        public override EventType EventType => EventType.RoomParticipantDisconnected;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            if (SourceParticipant == null) return;
            var participantState =  ParticipantState.Disconnected;
            var updateParticipantCommand = new UpdateParticipantStatusAndRoomCommand(SourceConference.Id, SourceParticipant.Id,
                participantState, null, null);
            await CommandHandler.Handle(updateParticipantCommand);
            var removeFromRoomCommand =
                new RemoveParticipantFromInterpreterRoomCommand(SourceInterpreterRoom.Id, SourceParticipant.Id);
            await CommandHandler.Handle(removeFromRoomCommand);
            
            if (SourceConference.State != ConferenceState.Closed)
            {
                await AddDisconnectedTask();
            }
        }
        
        private async Task AddDisconnectedTask()
        {
            var taskType = SourceParticipant.IsJudge() ? TaskType.Judge : TaskType.Participant;
            var disconnected = new AddTaskCommand(SourceConference.Id, SourceParticipant.Id, "Disconnected", taskType);

            await CommandHandler.Handle(disconnected);
        }
    }
}
