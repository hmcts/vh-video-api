using Microsoft.Extensions.Logging;
using System.Linq;
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
    public class TransferEventHandler : EventHandlerBase<TransferEventHandler>
    {
        private readonly IConsultationService _consultationService;

        public TransferEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<TransferEventHandler> logger, IConsultationService consultationService) : base(
            queryHandler, commandHandler, logger)
        {
            _consultationService = consultationService;
        }

        public override EventType EventType => EventType.Transfer;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            _logger.LogInformation("Transfer callback received - {ConferenceId} - {ParticipantId}/{ParticipantRoomId} - {FromRoom} {FromRoomLabel} - {ToRoom} {ToRoomLabel}",
                SourceConference.Id, callbackEvent.ParticipantId, callbackEvent.ParticipantRoomId, callbackEvent.TransferFrom, callbackEvent.TransferredFromRoomLabel, callbackEvent.TransferTo, callbackEvent.TransferredToRoomLabel);
            
            var participantStatus = DeriveParticipantStatusForTransferEvent(callbackEvent);

            var command =
                new UpdateParticipantStatusAndRoomCommand(SourceConference.Id, SourceParticipant.Id, participantStatus,
                    callbackEvent.TransferTo, callbackEvent.TransferredToRoomLabel);

            await CommandHandler.Handle(command);

            if (!callbackEvent.TransferredFromRoomLabel.ToLower().Contains("consultation") || callbackEvent.TransferTo == RoomType.HearingRoom)
            {
                return;
            }

            var roomQuery = new GetConsultationRoomByIdQuery(SourceConference.Id, callbackEvent.TransferredFromRoomLabel);
            var room = await QueryHandler.Handle<GetConsultationRoomByIdQuery, ConsultationRoom>(roomQuery);
            if (room == null)
            {
                _logger.LogError("Unable to find room {roomLabel} in conference {conferenceId}", callbackEvent.TransferredFromRoomLabel, SourceConference.Id);
            }
            // If room is live and empty, transfer all endpoints to waiting room
            else if (room.Status == RoomStatus.Live && !room.RoomParticipants.Any())
            {
                foreach (var endpoint in room.RoomEndpoints)
                {
                    await _consultationService.EndpointTransferToRoomAsync(SourceConference.Id, endpoint.EndpointId, RoomType.WaitingRoom.ToString());
                }
            }
            // If the room has endpoints connected, if none of the participants left in the room are participants the endpoint is linked to, transfer to waiting room
            else if (room.RoomEndpoints.Count != 0 && callbackEvent.Endpoints.Any())
               foreach (var endpoint in callbackEvent.Endpoints.Where(endpoint => room.RoomEndpoints.Exists(re => re.EndpointId == endpoint.Id) &&
                            !room.RoomParticipants.Exists(x => endpoint.LinkedParticipantIds.Contains(x.ParticipantId))))
                   await _consultationService.EndpointTransferToRoomAsync(SourceConference.Id, endpoint.Id, RoomType.WaitingRoom.ToString());
        }

        private ParticipantState DeriveParticipantStatusForTransferEvent(CallbackEvent callbackEvent)
        {
            if (!callbackEvent.TransferTo.HasValue && callbackEvent.TransferredToRoomLabel.ToLower().Contains("consultation"))
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
                    throw new RoomTransferException(callbackEvent.TransferredFromRoomLabel,
                        callbackEvent.TransferredToRoomLabel);
            }
        }
    }
}
