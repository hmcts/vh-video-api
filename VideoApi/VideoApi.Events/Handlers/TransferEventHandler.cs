using System.Linq;
using Microsoft.Extensions.Logging;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Events.Exceptions;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using VideoApi.Services.Contracts;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Events.Handlers
{
    public class TransferEventHandler(
        IQueryHandler queryHandler,
        ICommandHandler commandHandler,
        ILogger<TransferEventHandler> logger,
        IConsultationService consultationService)
        : EventHandlerBase<TransferEventHandler>(queryHandler, commandHandler, logger)
    {
        public override EventType EventType => EventType.Transfer;
        
        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            Logger.LogInformation("Transfer callback received - {ConferenceId} - {ParticipantId}/{ParticipantRoomId} - {FromRoom} {FromRoomLabel} - {ToRoom} {ToRoomLabel}",
                SourceConference.Id, callbackEvent.ParticipantId, callbackEvent.ParticipantRoomId, callbackEvent.TransferFrom, callbackEvent.TransferredFromRoomLabel, callbackEvent.TransferTo, callbackEvent.TransferredToRoomLabel);
            
            var participantStatus = DeriveParticipantStatusForTransferEvent(callbackEvent);

            
            
            var command =
                new UpdateParticipantStatusAndRoomCommand(SourceConference.Id, SourceParticipant.Id, participantStatus,
                    callbackEvent.TransferTo, callbackEvent.TransferredToRoomLabel);

            await CommandHandler.Handle(command);

            if (!callbackEvent.TransferredFromRoomLabel.Contains("consultation", System.StringComparison.CurrentCultureIgnoreCase) 
                || callbackEvent.TransferTo == RoomType.HearingRoom)
            {
                return;
            }

            var roomQuery = new GetConsultationRoomByIdQuery(SourceConference.Id, callbackEvent.TransferredFromRoomLabel);
            var room = await QueryHandler.Handle<GetConsultationRoomByIdQuery, ConsultationRoom>(roomQuery);
            if (room == null)
            {
                Logger.LogError("Unable to find room {roomLabel} in conference {conferenceId}", callbackEvent.TransferredFromRoomLabel, SourceConference.Id);
            }
            else if (room.Status == RoomStatus.Live && room.RoomParticipants.Count == 0)
            {
                foreach (var endpoint in room.RoomEndpoints)
                {
                    await consultationService.EndpointTransferToRoomAsync(SourceConference.Id, endpoint.EndpointId, RoomType.WaitingRoom.ToString());
                }
            }
            else if (room.RoomEndpoints.Count != 0)
            {
                var participantsEndpoints = SourceConference.GetEndpoints().Where(x => x.DefenceAdvocate?.Equals(SourceParticipant.Username, System.StringComparison.OrdinalIgnoreCase) ?? false).Select(x => x.Id).ToList();
                foreach (var endpoint in room.RoomEndpoints.Where(roomEndpoint => participantsEndpoints.Contains(roomEndpoint.EndpointId)))
                {
                    await consultationService.EndpointTransferToRoomAsync(SourceConference.Id, endpoint.EndpointId, RoomType.WaitingRoom.ToString());
                }
            }
        }
        
        private static ParticipantState DeriveParticipantStatusForTransferEvent(CallbackEvent callbackEvent)
        {
            if (!callbackEvent.TransferTo.HasValue && callbackEvent.TransferredToRoomLabel.Contains("consultation", System.StringComparison.CurrentCultureIgnoreCase))
            {
                return ParticipantState.InConsultation;
            }

            switch (callbackEvent.TransferTo)
            {
                case RoomType.ConsultationRoom:
                    return ParticipantState.InConsultation;
                case RoomType.WaitingRoom:
                    return ParticipantState.Available;
                case RoomType.HearingRoom:
                    return ParticipantState.InHearing;
                default:
                    throw new RoomTransferException(callbackEvent.TransferredFromRoomLabel, callbackEvent.TransferredToRoomLabel, callbackEvent.TransferTo);
            }
        }
    }
}
