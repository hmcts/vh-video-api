using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VideoApi.Contract.Requests;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Services.Contracts;
using VideoApi.Services.Kinly;
using Task = System.Threading.Tasks.Task;

namespace Video.API.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly IKinlyApiClient _kinlyApiClient;
        private readonly ILogger<ConsultationService> _logger;
        private readonly ICommandHandler _commandHandler;
        private readonly IQueryHandler _queryHandler;

        public ConsultationService(IKinlyApiClient kinlyApiClient, ILogger<ConsultationService> logger, ICommandHandler commandHandler, IQueryHandler queryHandler)
        {
            _kinlyApiClient = kinlyApiClient;
            _logger = logger;
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
        }
        
        public async Task<Room> GetAvailableConsultationRoomAsync(StartConsultationRequest request)
        {
            try
            {
                var query = new GetAvailableRoomByRoomTypeQuery(request.RoomType, request.ConferenceId);
                var listOfRooms = await _queryHandler.Handle<GetAvailableRoomByRoomTypeQuery, List<Room>>(query);
                var room = listOfRooms.FirstOrDefault(x => x.Type.Equals(request.RoomType));
                if (room == null)
                {
                    var consultationRoomParams = new CreateConsultationRoomParams
                    {
                        Room_label_prefix = request.RoomType.ToString()
                    };
                    var createConsultationRoomResponse =
                        await CreateConsultationRoomAsync(request.ConferenceId.ToString(),
                            consultationRoomParams);
                    var createRoomCommand = new CreateRoomCommand(request.ConferenceId, createConsultationRoomResponse.Room_label,
                        request.RoomType);
                    await _commandHandler.Handle(createRoomCommand);
                    room = new Room(request.ConferenceId, createConsultationRoomResponse.Room_label, request.RoomType);
                }
                
                return room;
            }
            catch (ConferenceNotFoundException ex)
            {
                _logger.LogError("Cannot create consultation for conference: {conferenceId} as the conference does not exist. Error: {ex}",
                    request.ConferenceId, ex);
                throw;
            }
        }
        
        public async Task<IActionResult> TransferParticipantToConsultationRoomAsync(StartConsultationRequest request, Room room)
        {
            Participant participant;
            try
            {
                var conference =
                    await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(
                        new GetConferenceByIdQuery(request.ConferenceId));
                participant = conference.GetParticipants().Single(x => x.Id == request.RequestedBy);
            }
            catch (ParticipantNotFoundException ex)
            {
                _logger.LogError(
                    "Cannot create consultation with participant: {participantId} as the participant does not exist. Error message: ",
                    request.RequestedBy, ex);
                return new NotFoundResult();
            }

            await TransferParticipantAsync(request.ConferenceId, request.RequestedBy,
                participant.GetCurrentRoom().ToString(), room.Label);

            return new AcceptedResult();
        }

        private Task<CreateConsultationRoomResponse> CreateConsultationRoomAsync(string virtualCourtRoomId, CreateConsultationRoomParams createConsultationRoomParams)
        {
            _logger.LogTrace("Creating a consultation for VirtualCourtRoomId: {virtualCourtRoomId} with prefix {createConsultationRoomParamsPrefix}",
                virtualCourtRoomId, createConsultationRoomParams.Room_label_prefix);

            try
            {
                return _kinlyApiClient.CreateConsultationRoomAsync(virtualCourtRoomId, createConsultationRoomParams);
            }
            catch (KinlyApiException e)
            {
                _logger.LogError("Unable to create a consultation room for VirtualCourtRoomId: {virtualCourtRoomId}, with exception message: {e}", 
                    virtualCourtRoomId, e);
                throw;
            }
        }
        
        private async Task TransferParticipantAsync(Guid conferenceId, Guid participantId, string fromRoom, string toRoom)
        {
            _logger.LogTrace(
                "Transferring participant {participantId} from {fromRoom} to {toRoom} in conference: {conferenceId}",
                participantId, fromRoom, toRoom, conferenceId);

            var request = new TransferParticipantParams
            {
                From = fromRoom,
                To = toRoom,
                Part_id = participantId.ToString()
            };
            try
            {
                await _kinlyApiClient.TransferParticipantAsync(conferenceId.ToString(), request);
            }
            catch (KinlyApiException e)
            {
                _logger.LogError("Unable to start a transfer participant: {participantId} for ConferenceId: {{conferenceId}}, with exception message: {{e}}", 
                    participantId, conferenceId, e);
                throw;
            }
        }
    }
}
