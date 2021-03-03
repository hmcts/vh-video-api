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

            if (!callbackEvent.TransferredFromRoomLabel.ToLower().Contains("consultation") || callbackEvent.TransferTo == RoomType.HearingRoom)
            {
                return;
            }

            var roomQuery = new GetRoomByIdQuery(SourceConference.Id, callbackEvent.TransferredFromRoomLabel);
            var room = await QueryHandler.Handle<GetRoomByIdQuery, Room>(roomQuery);
            if (room == null)
            {
                _logger.LogError("Unable to find room {roomLabel} in conference {conferenceId}", callbackEvent.TransferredFromRoomLabel, SourceConference.Id);
            }
            else if (room.Status == RoomStatus.Live && !room.RoomParticipants.Any())
            {
                foreach (var endpoint in room.RoomEndpoints)
                {
                    await _consultationService.EndpointTransferToRoomAsync(SourceConference.Id, endpoint.EndpointId, RoomType.WaitingRoom.ToString());
                }
            }
            else if (room.RoomEndpoints.Any())
            {
                var participantsEndpoints = SourceConference.GetEndpoints().Where(x => x.DefenceAdvocate.Equals(SourceParticipant.Username, System.StringComparison.OrdinalIgnoreCase)).Select(x => x.Id).ToList();
                foreach (var endpoint in room.RoomEndpoints.Where(roomEndpoint => participantsEndpoints.Contains(roomEndpoint.EndpointId)))
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
