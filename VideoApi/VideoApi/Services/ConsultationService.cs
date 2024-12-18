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
using VideoApi.Services.Clients;
using VideoApi.Services.Contracts;
using Supplier = VideoApi.Domain.Enums.Supplier;
using Task = System.Threading.Tasks.Task;
using VirtualCourtRoomType = VideoApi.Domain.Enums.VirtualCourtRoomType;

namespace VideoApi.Services
{
    public class ConsultationService(
        ISupplierPlatformServiceFactory supplierPlatformServiceFactory,
        ILogger<ConsultationService> logger,
        ICommandHandler commandHandler,
        IQueryHandler queryHandler)
        : IConsultationService
    {
        public async Task<ConsultationRoom> CreateNewConsultationRoomAsync(Guid conferenceId, VirtualCourtRoomType roomType = VirtualCourtRoomType.Participant, bool locked = true)
        {
            var consultationRoomParams = new CreateConsultationRoomParams
            {
                Room_label_prefix = roomType.ToString()
            };
            var conference =
                await queryHandler.Handle<GetConferenceByIdQuery, Conference>(
                    new GetConferenceByIdQuery(conferenceId));
            var createConsultationRoomResponse = await CreateConsultationRoomAsync(conferenceId.ToString(), consultationRoomParams, conference.Supplier);
            var createRoomCommand = new CreateConsultationRoomCommand(conferenceId, createConsultationRoomResponse.Room_label, roomType, locked);
            await commandHandler.Handle(createRoomCommand);
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
                var conference =
                    await queryHandler.Handle<GetConferenceByIdQuery, Conference>(
                        new GetConferenceByIdQuery(conferenceId));
                var createConsultationRoomResponse =
                    await CreateConsultationRoomAsync(conferenceId.ToString(),
                        consultationRoomParams, conference.Supplier);
                var createRoomCommand = new CreateConsultationRoomCommand(conferenceId,
                    createConsultationRoomResponse.Room_label,
                    roomType,
                    false);
                await commandHandler.Handle(createRoomCommand);
                room = new ConsultationRoom(conferenceId, createConsultationRoomResponse.Room_label, roomType, false);
            }

            return room;
        }
        
        
        public async Task EndpointTransferToRoomAsync(Guid conferenceId, Guid endpointId, string room)
        {
            var conference =
                await queryHandler.Handle<GetConferenceByIdQuery, Conference>(
                    new GetConferenceByIdQuery(conferenceId));
            var endpint = conference.GetEndpoints().Single(x => x.Id == endpointId);

            await TransferParticipantAsync(conferenceId, endpointId.ToString(), endpint.GetCurrentRoom(), room, false, conference.Supplier);
        }
        
        public async Task ParticipantTransferToRoomAsync(Guid conferenceId, Guid participantId, string room)
        {
            var conference =
                await queryHandler.Handle<GetConferenceByIdQuery, Conference>(
                    new GetConferenceByIdQuery(conferenceId));
            var participant = conference.GetParticipants().Single(x => x.Id == participantId);
            var kinlyParticipantId = participant.GetParticipantRoom()?.Id.ToString() ?? participantId.ToString(); 
            await TransferParticipantAsync(conferenceId, kinlyParticipantId, participant.GetCurrentRoom(), room, participant.IsHost(), conference.Supplier);
        }
        
        public async Task LeaveConsultationAsync(Guid conferenceId, Guid participantId, string fromRoom, string toRoom)
        {
            var conference =
                await queryHandler.Handle<GetConferenceByIdQuery, Conference>(
                    new GetConferenceByIdQuery(conferenceId));
            
            var participant = conference.GetParticipants().Single(x => x.Id == participantId);

            var kinlyParticipantId = participant.GetParticipantRoom()?.Id.ToString() ?? participantId.ToString(); 
            await TransferParticipantAsync(conferenceId, kinlyParticipantId, fromRoom, toRoom, participant.IsHost(), conference.Supplier);

            var lastLinkedParticipant =
                await RetrieveLastParticipantIfLinkedAndLeftAlone(conference, participant, fromRoom); 
            if (lastLinkedParticipant != null)
            {
                await TransferParticipantAsync(conferenceId, lastLinkedParticipant.GetParticipantRoom()?.Id.ToString(), fromRoom, toRoom, lastLinkedParticipant.IsHost(), conference.Supplier);
            }
        }
        
        private async Task<IEnumerable<ConsultationRoom>> GetLiveRooms<TQuery>(TQuery query) where TQuery : IQuery
        {
            var liveRooms = new List<ConsultationRoom>();
            var listOfRooms = await queryHandler.Handle<TQuery, List<ConsultationRoom>>(query);
            foreach (var consultationRoom in listOfRooms)
            {
                if (consultationRoom.RoomParticipants.Count == 0)
                {
                    var closeRoomCommand = new CloseConsultationRoomCommand(consultationRoom.Id);
                    await commandHandler.Handle(closeRoomCommand);
                    consultationRoom.CloseRoom();
                    continue;
                }
                
                liveRooms.Add(consultationRoom);
            }

            return liveRooms;
        }
        
        
        private async Task<CreateConsultationRoomResponse> CreateConsultationRoomAsync(string virtualCourtRoomId,
            CreateConsultationRoomParams createConsultationRoomParams, Supplier supplier)
        {
            var supplierPlatformService = supplierPlatformServiceFactory.Create(supplier);
            var supplierApiClient = supplierPlatformService.GetHttpClient();
            var response = await supplierApiClient.CreateConsultationRoomAsync(virtualCourtRoomId, createConsultationRoomParams);
            logger.LogInformation(
                "Created a consultation in {VirtualCourtRoomId} with prefix {CreateConsultationRoomParamsPrefix} - Response {RoomLabel}",
                virtualCourtRoomId, createConsultationRoomParams.Room_label_prefix, response?.Room_label);

            return response;
        }
        
        private async Task<Participant> RetrieveLastParticipantIfLinkedAndLeftAlone(Conference conference, ParticipantBase participant, string fromRoomLabel)
        {
            if (participant.GetParticipantRoom() != null)
            {
                return null;
            }

            var roomQuery = new GetConsultationRoomByIdQuery(conference.Id, fromRoomLabel);
            var consultationRoom =
                await queryHandler.Handle<GetConsultationRoomByIdQuery, ConsultationRoom>(roomQuery);
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
        
        private async Task TransferParticipantAsync(Guid conferenceId, string participantId, string fromRoom, string toRoom, bool isHost, Supplier supplier)
        {
            logger.LogInformation(
                "Transferring participant {ParticipantId} from {FromRoom} to {ToRoom} in conference: {ConferenceId}",
                participantId, fromRoom, toRoom, conferenceId);
            var supplierPlatformService = supplierPlatformServiceFactory.Create(supplier);
            await supplierPlatformService.TransferParticipantAsync(conferenceId, participantId, fromRoom, toRoom, isHost ? ConferenceRole.Host : ConferenceRole.Guest);
        }
    }
}
