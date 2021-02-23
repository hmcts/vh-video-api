using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
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
            var participantStatus = DeriveParticipantStatusForTransferEvent(callbackEvent);

            var command =
                new UpdateParticipantStatusAndRoomCommand(SourceConference.Id, SourceParticipant.Id, participantStatus,
                    callbackEvent.TransferTo, callbackEvent.TransferredToRoomLabel);
            await CommandHandler.Handle(command);

            if (!callbackEvent.TransferredFromRoomLabel.ToLower().Contains("consultation"))
            {
                return;
            }

            var roomQuery = new GetRoomByIdQuery(SourceConference.Id, callbackEvent.TransferredFromRoomLabel);
            var room = await QueryHandler.Handle<GetRoomByIdQuery, Room>(roomQuery);
            if (!room.RoomParticipants.Any())
            {
                foreach (var endpoint in room.RoomEndpoints)
                {
                    await _consultationService.EndpointTransferToRoomAsync(SourceConference.Id, endpoint.EndpointId, RoomType.WaitingRoom.ToString());
                }
            }
        }

        private ParticipantState DeriveParticipantStatusForTransferEvent(CallbackEvent callbackEvent)
        {
            if (!callbackEvent.TransferTo.HasValue && callbackEvent.TransferredToRoomLabel.ToLower().Contains("consultation"))
            {
                return ParticipantState.InConsultation;
            }
            
            
            if (callbackEvent.TransferFrom == RoomType.WaitingRoom &&
                callbackEvent.TransferTo == RoomType.ConsultationRoom)
                return ParticipantState.InConsultation;

            if ((callbackEvent.TransferFrom == RoomType.ConsultationRoom || 
                 callbackEvent.TransferredFromRoomLabel.ToLower().Contains("consultation")) &&
                callbackEvent.TransferTo == RoomType.WaitingRoom)
                return ParticipantState.Available;

            if (callbackEvent.TransferFrom == RoomType.ConsultationRoom &&
                callbackEvent.TransferTo == RoomType.HearingRoom)
                return ParticipantState.InHearing;

            switch (callbackEvent.TransferFrom)
            {
                case RoomType.WaitingRoom when callbackEvent.TransferTo == RoomType.HearingRoom:
                    return ParticipantState.InHearing;
                case RoomType.HearingRoom when callbackEvent.TransferTo == RoomType.WaitingRoom:
                    return ParticipantState.Available;
                default:
                    throw new RoomTransferException(callbackEvent.TransferredFromRoomLabel,
                        callbackEvent.TransferredToRoomLabel);
            }
        }
    }
}
