using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Contracts;
using VideoApi.Services.Kinly;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly IKinlyApiClient _kinlyApiClient;
        private readonly ILogger<ConsultationService> _logger;
        private readonly ICommandHandler _commandHandler;
        private readonly IQueryHandler _queryHandler;

        public ConsultationService(IKinlyApiClient kinlyApiClient, ILogger<ConsultationService> logger,
            ICommandHandler commandHandler, IQueryHandler queryHandler)
        {
            _kinlyApiClient = kinlyApiClient;
            _logger = logger;
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
        }

        public async Task<ConsultationRoom> CreateNewConsultationRoomAsync(Guid conferenceId, VirtualCourtRoomType roomType = VirtualCourtRoomType.Participant, bool locked = true)
        {
            var consultationRoomParams = new CreateConsultationRoomParams
            {
                Room_label_prefix = roomType.ToString()
            };
            var createConsultationRoomResponse = await CreateConsultationRoomAsync(conferenceId.ToString(), consultationRoomParams);
            var createRoomCommand = new CreateConsultationRoomCommand(conferenceId, createConsultationRoomResponse.Room_label, roomType, locked);
            await _commandHandler.Handle(createRoomCommand);
            var room = new ConsultationRoom(conferenceId, createConsultationRoomResponse.Room_label, roomType, locked);
            return room;
        }

        public async Task<ConsultationRoom> GetAvailableConsultationRoomAsync(Guid conferenceId, VirtualCourtRoomType roomType)
        {
            var getAvailableConsultationRoomsByRoomTypeQuery = new GetAvailableConsultationRoomsByRoomTypeQuery(roomType, conferenceId);
            var liveRooms = await GetLiveRooms(getAvailableConsultationRoomsByRoomTypeQuery);
            
            var room = liveRooms?.FirstOrDefault(x => x.Type.Equals(roomType));
            if (room == null)
            {
                var consultationRoomParams = new CreateConsultationRoomParams
                {
                    Room_label_prefix = roomType.ToString()
                };
                var createConsultationRoomResponse =
                    await CreateConsultationRoomAsync(conferenceId.ToString(),
                        consultationRoomParams);
                var createRoomCommand = new CreateConsultationRoomCommand(conferenceId,
                    createConsultationRoomResponse.Room_label,
                    roomType,
                    false);
                await _commandHandler.Handle(createRoomCommand);
                room = new ConsultationRoom(conferenceId, createConsultationRoomResponse.Room_label, roomType, false);
            }

            return room;
        }

        private async Task<IEnumerable<ConsultationRoom>> GetLiveRooms<TQuery>(TQuery query) where TQuery : IQuery
        {
            List<ConsultationRoom> liveRooms = new List<ConsultationRoom>();
            var listOfRooms = await _queryHandler.Handle<TQuery, List<ConsultationRoom>>(query);
            foreach (var consultationRoom in listOfRooms)
            {
                if (!consultationRoom.RoomParticipants.Any())
                {
                    var closeRoomCommand = new CloseConsultationRoomCommand(consultationRoom.Id);
                    await _commandHandler.Handle(closeRoomCommand);
                    consultationRoom.CloseRoom();
                    continue;
                }
                
                liveRooms.Add(consultationRoom);
            }

            return liveRooms;
        }


        private async Task<CreateConsultationRoomResponse> CreateConsultationRoomAsync(string virtualCourtRoomId,
            CreateConsultationRoomParams createConsultationRoomParams)
        {
            var response = await _kinlyApiClient.CreateConsultationRoomAsync(virtualCourtRoomId, createConsultationRoomParams);
            _logger.LogInformation(
                "Created a consultation in {VirtualCourtRoomId} with prefix {CreateConsultationRoomParamsPrefix} - Response {RoomLabel}",
                virtualCourtRoomId, createConsultationRoomParams.Room_label_prefix, response?.Room_label);

            return response;
        }


        public async Task EndpointTransferToRoomAsync(Guid conferenceId, Guid endpointId, string room)
        {
            var conference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(
                    new GetConferenceByIdQuery(conferenceId));
            var endpint = conference.GetEndpoints().Single(x => x.Id == endpointId);

            await TransferParticipantAsync(conferenceId, endpointId.ToString(), endpint.GetCurrentRoom(), room);
        }

        public async Task ParticipantTransferToRoomAsync(Guid conferenceId, Guid participantId, string room)
        {
            var conference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(
                    new GetConferenceByIdQuery(conferenceId));
            var participant = conference.GetParticipants().Single(x => x.Id == participantId);

            var kinlyParticipantId = participant.GetParticipantRoom()?.Id.ToString() ?? participantId.ToString(); 
            await TransferParticipantAsync(conferenceId, kinlyParticipantId, participant.GetCurrentRoom(), room);
        }
        
        public async Task LeaveConsultationAsync(Guid conferenceId, Guid participantId, string fromRoom, string toRoom)
        {
            var conference =
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(
                    new GetConferenceByIdQuery(conferenceId));
            
            var participant = conference.GetParticipants().Single(x => x.Id == participantId);

            var kinlyParticipantId = participant.GetParticipantRoom()?.Id.ToString() ?? participantId.ToString(); 
            await TransferParticipantAsync(conferenceId, kinlyParticipantId, fromRoom, toRoom);

            var lastLinkedParticipant =
                await RetrieveLastParticipantIfLinkedAndLeftAlone(conference, participant, fromRoom); 
            if (lastLinkedParticipant != null)
            {
                await TransferParticipantAsync(conferenceId, lastLinkedParticipant.GetParticipantRoom()?.Id.ToString(), fromRoom, toRoom);
            }
        }

        private async Task<Participant> RetrieveLastParticipantIfLinkedAndLeftAlone(Conference conference, ParticipantBase participant,
            string fromRoomLabel)
        {
            if (participant.GetParticipantRoom() != null)
            {
                return null;
            }

            var roomQuery = new GetConsultationRoomByIdQuery(conference.Id, fromRoomLabel);
            var consultationRoom =
                await _queryHandler.Handle<GetConsultationRoomByIdQuery, ConsultationRoom>(roomQuery);
            var remainingParticipants =
                consultationRoom.RoomParticipants.Where(x => x.ParticipantId != participant.Id).ToList();
            if (remainingParticipants.Count != 2)
            {
                return null;
            }

            var participantIds = remainingParticipants.Select(x => x.ParticipantId);

            var firstRemaining = conference.Participants.First(x => participantIds.Contains(x.Id));
            return firstRemaining is Participant && ((Participant)firstRemaining).LinkedParticipants.Any(x => participantIds.Contains(x.LinkedId))
                ? ((Participant)firstRemaining): null;
        }

        private async Task TransferParticipantAsync(Guid conferenceId, string participantId, string fromRoom,
            string toRoom)
        {
            _logger.LogInformation(
                "Transferring participant {ParticipantId} from {FromRoom} to {ToRoom} in conference: {ConferenceId}",
                participantId, fromRoom, toRoom, conferenceId);

            var request = new TransferParticipantParams
            {
                From = fromRoom,
                To = toRoom,
                Part_id = participantId
            };

            await _kinlyApiClient.TransferParticipantAsync(conferenceId.ToString(), request);
        }

    }
}
