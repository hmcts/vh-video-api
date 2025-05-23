using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using VideoApi.Common;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Events.Exceptions;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using VideoApi.Services.Contracts;
using Task = System.Threading.Tasks.Task;
using VideoApi.Common.Logging;

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
            if(SourceParticipant == null) throw new ParticipantNotFoundException(callbackEvent.ParticipantId);
            BadRequestException.ThrowIfNull(callbackEvent.TransferredFromRoomLabel);
            BadRequestException.ThrowIfNull(callbackEvent.TransferredToRoomLabel);
            Logger.LogTransferCallbackReceived(SourceConference.Id, callbackEvent.ParticipantId, callbackEvent.ParticipantRoomId, callbackEvent.TransferFrom.ToString(), callbackEvent.TransferredFromRoomLabel, callbackEvent.TransferTo.ToString(), callbackEvent.TransferredToRoomLabel);
            
            var participantStatus = DeriveParticipantStatusForTransferEvent(callbackEvent);

            var command = new UpdateParticipantStatusAndRoomCommand(SourceConference.Id, SourceParticipant.Id, participantStatus, callbackEvent.TransferTo, callbackEvent.TransferredToRoomLabel);
            await CommandHandler.Handle(command);

            if (!callbackEvent.TransferredFromRoomLabel.Contains("consultation", StringComparison.CurrentCultureIgnoreCase) 
                || callbackEvent.TransferTo == RoomType.HearingRoom)
            {
                return;
            }

            var roomQuery = new GetConsultationRoomByIdQuery(SourceConference.Id, callbackEvent.TransferredFromRoomLabel);
            var room = await QueryHandler.Handle<GetConsultationRoomByIdQuery, ConsultationRoom>(roomQuery);
            if (room == null)
            {
                Logger.LogRoomNotFound(new RoomNotFoundException(SourceConference.Id, callbackEvent.TransferredFromRoomLabel), callbackEvent.TransferredFromRoomLabel, SourceConference.Id);
            }
            else if (room.Status == RoomStatus.Live && room.RoomParticipants.Count == 0)
            {                
                Logger.LogNoParticipantsLeftInRoom(room.Label);
                foreach (var endpoint in room.RoomEndpoints)
                {
                    await consultationService.EndpointTransferToRoomAsync(SourceConference.Id, endpoint.EndpointId, RoomType.WaitingRoom.ToString());
                }
            }
            else if (room.RoomEndpoints.Count != 0)
            {
                await HandleLinkedEndpoints(room);
            }
        }

        private async Task HandleLinkedEndpoints(ConsultationRoom room)
        {
            //Get all endpoints this participant is linked to
            var endpointsLinked = SourceConference
                .GetEndpoints()
                .Where(x => x.ParticipantsLinked?.Any(pl => pl.Id == SourceParticipant.Id) ?? false)
                .ToList();
            
            foreach (var endpoint in endpointsLinked.Where(ep => room.RoomEndpoints.Any(e => e.EndpointId == ep.Id)))
            {
                // Check if this endpoint has any of other participants linked to it still in the room
                var linkedParticipantsStillInRoom = room.RoomParticipants.Select(rp => rp.ParticipantId)
                    .Intersect(endpoint.ParticipantsLinked?
                        .Select(pl => pl.Id) ?? new List<Guid>())
                    .ToList();
                
                // If no other participants linked to this endpoint are still in the room, transfer it to waiting room
                if (linkedParticipantsStillInRoom.Count == 0)
                {
                    Logger.LogNoOtherParticipantsLinkedToEndpoint(endpoint.Id);
                    await consultationService.EndpointTransferToRoomAsync(SourceConference.Id, endpoint.Id, RoomType.WaitingRoom.ToString());
                }
            }
        }

        private static ParticipantState DeriveParticipantStatusForTransferEvent(CallbackEvent callbackEvent)
        {
            if (!callbackEvent.TransferTo.HasValue && callbackEvent.TransferredToRoomLabel.Contains("consultation", StringComparison.CurrentCultureIgnoreCase))
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
