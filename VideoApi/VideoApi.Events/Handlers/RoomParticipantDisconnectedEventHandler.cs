using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using VideoApi.Services.Contracts;

namespace VideoApi.Events.Handlers
{
    public class RoomParticipantDisconnectedEventHandler : EventHandlerBase<RoomParticipantDisconnectedEventHandler>
    {
        private readonly IConsultationService _consultationService;

        public RoomParticipantDisconnectedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler,
            ILogger<RoomParticipantDisconnectedEventHandler> logger, IConsultationService consultationService) : base(
            queryHandler, commandHandler, logger)
        {
            _consultationService = consultationService;
        }

        public override EventType EventType => EventType.RoomParticipantDisconnected;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            if (SourceParticipant == null)
            {
                return;
            }

            await ReturnRoomParticipantToWaitingRoom();
            
            var participantState =  ParticipantState.Disconnected;
            var updateParticipantCommand = new UpdateParticipantStatusAndRoomCommand(SourceConference.Id, SourceParticipant.Id,
                participantState, null, null);
            await CommandHandler.Handle(updateParticipantCommand);
            var removeFromRoomCommand =
                new RemoveParticipantFromParticipantRoomCommand(SourceParticipantRoom.Id, SourceParticipant.Id);
            await CommandHandler.Handle(removeFromRoomCommand);
            
            if (!SourceConference.IsClosed())
            {
                await AddDisconnectedTask();
            }
            
            _logger.LogInformation("Room Participant Disconnected callback received - {ConferenceId}/{ParticipantId} - {ParticipantState} - {Room} {RoomLabel} - {SourceRoom} - {Tags}",
                SourceConference.Id, SourceParticipant.Id, participantState, null, null, SourceParticipantRoom.Id, new [] {"VIH-7730", "HearingEvent"});
        }

        private async Task ReturnRoomParticipantToWaitingRoom()
        {
            if (SourceParticipant.State == ParticipantState.InConsultation)
            {
                var currentConsultationRoom = SourceParticipant.GetCurrentRoom();
                await _consultationService.LeaveConsultationAsync(SourceConference.Id, SourceParticipant.Id,
                    currentConsultationRoom, RoomType.WaitingRoom.ToString());
            }
        }
        
        private Task AddDisconnectedTask()
        {
            var taskType = SourceParticipant.IsJudge() ? TaskType.Judge : TaskType.Participant;
            var disconnected = new AddTaskCommand(SourceConference.Id, SourceParticipant.Id, "Disconnected", taskType);

            return CommandHandler.Handle(disconnected);
        }
    }
}
