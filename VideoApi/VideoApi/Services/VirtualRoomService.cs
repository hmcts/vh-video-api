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
    public class VirtualRoomService : IVirtualRoomService
    {
        private readonly IKinlyApiClient _kinlyApiClient;
        private readonly ILogger<VirtualRoomService> _logger;
        private readonly ICommandHandler _commandHandler;
        private readonly IQueryHandler _queryHandler;

        private string InterpreterRoomPrefix => "Interpreter";
        private string PanelMemberRoomPrefix => "Panel Member";
        private string InterpreterSuffix => "_interpreter";

        public VirtualRoomService(IKinlyApiClient kinlyApiClient, ILogger<VirtualRoomService> logger,
            ICommandHandler commandHandler, IQueryHandler queryHandler)
        {
            _kinlyApiClient = kinlyApiClient;
            _logger = logger;
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
        }

        public Task<ParticipantRoom> GetOrCreateAnInterpreterVirtualRoom(Conference conference, Participant participant)
        {
            return GetOrCreateInterpreterRoom(conference, participant, VirtualCourtRoomType.Civilian);
        }

        public Task<ParticipantRoom> GetOrCreateAWitnessVirtualRoom(Conference conference, Participant participant)
        {
            return GetOrCreateInterpreterRoom(conference, participant, VirtualCourtRoomType.Witness);
        }

        public async Task<ParticipantRoom> GetOrCreateAJudicialVirtualRoom(Conference conference,
            Participant participant)
        {
            var conferenceId = conference.Id;
            var judicialRoom = await RetrieveJudicialRoomForConference(conferenceId);
            if (judicialRoom != null) return judicialRoom;

            judicialRoom = await CreateAVmrAndRoom(conference, 0, VirtualCourtRoomType.JudicialShared,
                KinlyRoomType.Panel_Member);
            return judicialRoom;
        }

        private async Task<ParticipantRoom> GetOrCreateInterpreterRoom(Conference conference, Participant participant,
            VirtualCourtRoomType type)
        {
            var conferenceId = conference.Id;
            var interpreterRooms = await RetrieveAllInterpreterRooms(conferenceId);

            var interpreterRoom = GetInterpreterRoomForParticipant(conferenceId, interpreterRooms, participant, type);
            if (interpreterRoom != null) return interpreterRoom;
            var count = interpreterRooms
                .Select(x => int.TryParse(x.Label.Replace(InterpreterRoomPrefix, string.Empty), out var n) ? n : 0)
                .DefaultIfEmpty()
                .Max();
            interpreterRoom = await CreateAVmrAndRoom(conference, count, type, KinlyRoomType.Interpreter);

            return interpreterRoom;
        }

        private async Task<IReadOnlyCollection<ParticipantRoom>> RetrieveAllInterpreterRooms(Guid conferenceId)
        {
            var participantRoomsQuery =
                new GetParticipantRoomsForConferenceQuery(conferenceId);

            var interpreterRooms =
                await _queryHandler.Handle<GetParticipantRoomsForConferenceQuery, List<ParticipantRoom>>(
                    participantRoomsQuery);
            return interpreterRooms.Where(x => x.Label.StartsWith(InterpreterRoomPrefix)).ToList().AsReadOnly();
        }

        private async Task<ParticipantRoom> RetrieveJudicialRoomForConference(Guid conferenceId)
        {
            var participantRoomsQuery =
                new GetParticipantRoomsForConferenceQuery(conferenceId);

            var interpreterRooms =
                await _queryHandler.Handle<GetParticipantRoomsForConferenceQuery, List<ParticipantRoom>>(
                    participantRoomsQuery);
            return interpreterRooms.FirstOrDefault(x => x.Type == VirtualCourtRoomType.JudicialShared);
        }

        private async Task<ParticipantRoom> CreateAVmrAndRoom(Conference conference, int existingRooms,
            VirtualCourtRoomType roomType, KinlyRoomType kinlyRoomType)
        {
            _logger.LogInformation("Creating a new interpreter room for conference {Conference}", conference.Id);
            var roomId = await CreateInterpreterRoom(conference.Id, roomType);
            var ingestUrl = roomType == VirtualCourtRoomType.JudicialShared? null :
                                kinlyRoomType == KinlyRoomType.Interpreter? 
                                $"{conference.IngestUrl}/{roomId}/{InterpreterSuffix}": 
                                $"{conference.IngestUrl}/{roomId}";

            var vmr = await CreateVmr(conference, roomId, ingestUrl, roomType, existingRooms, kinlyRoomType);
            await UpdateRoomConnectionDetails(conference, roomId, vmr, ingestUrl);
            _logger.LogDebug("Updated room {Room} for conference {Conference} with joining details", roomId,
                conference.Id);

            return await GetUpdatedRoom(conference, roomId);
        }

        private async Task<long> CreateInterpreterRoom(Guid conferenceId, VirtualCourtRoomType type)
        {
            var createRoomCommand = new CreateParticipantRoomCommand(conferenceId, type);
            await _commandHandler.Handle(createRoomCommand);
            return createRoomCommand.NewRoomId;
        }

        private Task<BookedParticipantRoomResponse> CreateVmr(Conference conference, long roomId, string ingestUrl,
            VirtualCourtRoomType roomType, int existingRooms, KinlyRoomType kinlyRoomType)
        {
            var ingest = kinlyRoomType == KinlyRoomType.Panel_Member ? string.Empty : ingestUrl;
            var kinlyParticipantType = roomType == VirtualCourtRoomType.Witness ? "Witness" : "Civilian";
            var roomPrefix = kinlyRoomType == KinlyRoomType.Panel_Member
                ? PanelMemberRoomPrefix
                : InterpreterRoomPrefix; 
            
            var newRoomParams = new CreateParticipantRoomParams
            {
                Audio_recording_url = ingest,
                Participant_room_id = roomId.ToString(),
                Participant_type = kinlyParticipantType,
                Room_label_prefix = roomPrefix,
                Room_type = kinlyRoomType,
                Display_name = $"{roomPrefix}{existingRooms + 1}"
            };
            return _kinlyApiClient.CreateParticipantRoomAsync(conference.Id.ToString(), newRoomParams);
        }

        private ParticipantRoom GetInterpreterRoomForParticipant(Guid conferenceId,
            IReadOnlyCollection<ParticipantRoom> rooms, Participant participant,
            VirtualCourtRoomType roomType)
        {
            _logger.LogInformation(
                "Checking for an existing interpreter room for participant {Participant} in conference {Conference}",
                participant.Id, conferenceId);
            var participantIds = participant.LinkedParticipants.Select(lp => lp.LinkedId).ToList();
            participantIds.Add(participant.Id);

            var matchingRooms = participantIds
                .Select(l => rooms.SingleOrDefault(r => r.DoesParticipantExist(new RoomParticipant(l))))
                .Where(m => m?.Type == roomType)
                .ToList();

            var existingRoom = matchingRooms.FirstOrDefault();
            return existingRoom ?? rooms.FirstOrDefault(x => x.Type == roomType && !x.RoomParticipants.Any());
        }

        private Task UpdateRoomConnectionDetails(Conference conference, long roomId, BookedParticipantRoomResponse vmr,
            string ingestUrl)
        {
            var updateCommand = new UpdateParticipantRoomConnectionDetailsCommand(conference.Id, roomId, vmr.Room_label,
                ingestUrl, vmr.Uris.Pexip_node, vmr.Uris.Participant);
            return _commandHandler.Handle(updateCommand);
        }

        private async Task<ParticipantRoom> GetUpdatedRoom(Conference conference, long roomId)
        {
            var query = new GetParticipantRoomsForConferenceQuery(conference.Id);
            var listOfRooms =
                await _queryHandler.Handle<GetParticipantRoomsForConferenceQuery, List<ParticipantRoom>>(query);
            return listOfRooms.First(x => x.Id == roomId);
        }
    }
}
