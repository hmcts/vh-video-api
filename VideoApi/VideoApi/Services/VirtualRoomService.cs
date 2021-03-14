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

        private string RoomPrefix => "Interpreter";

        public VirtualRoomService(IKinlyApiClient kinlyApiClient, ILogger<VirtualRoomService> logger,
            ICommandHandler commandHandler, IQueryHandler queryHandler)
        {
            _kinlyApiClient = kinlyApiClient;
            _logger = logger;
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
        }

        public Task<InterpreterRoom> GetOrCreateAnInterpreterVirtualRoom(Conference conference, Participant participant)
        {
            return GetOrCreateInterpreterRoom(conference, participant, VirtualCourtRoomType.Civilian);
        }

        public Task<InterpreterRoom> GetOrCreateAWitnessVirtualRoom(Conference conference, Participant participant)
        {
            return GetOrCreateInterpreterRoom(conference, participant, VirtualCourtRoomType.Witness);
        }

        private async Task<InterpreterRoom> GetOrCreateInterpreterRoom(Conference conference, Participant participant,
            VirtualCourtRoomType type)
        {
            var conferenceId = conference.Id;
            var interpreterRooms = await RetrieveAllInterpreterRooms(conferenceId);

            var interpreterRoom = GetInterpreterRoomForParticipant(conferenceId, interpreterRooms, participant, type);
            if (interpreterRoom != null) return interpreterRoom;
            var count = interpreterRooms
                .Select(x => int.TryParse(x.Label.Replace(RoomPrefix, string.Empty), out var n) ? n : 0)
                .DefaultIfEmpty()
                .Max();
            interpreterRoom = await CreateAVmrAndRoom(conference, count, type);

            return interpreterRoom;
        }

        private async Task<IReadOnlyCollection<InterpreterRoom>> RetrieveAllInterpreterRooms(Guid conferenceId)
        {
            var interpreterRoomsQuery =
                new GetInterpreterRoomsForConferenceQuery(conferenceId);

            var interpreterRooms =
                await _queryHandler.Handle<GetInterpreterRoomsForConferenceQuery, List<InterpreterRoom>>(interpreterRoomsQuery);
            return interpreterRooms.Where(x => x.Label.StartsWith(RoomPrefix)).ToList().AsReadOnly();
        }

        private async Task<InterpreterRoom> CreateAVmrAndRoom(Conference conference, int existingRooms,
            VirtualCourtRoomType roomType)
        {
            _logger.LogInformation("Creating a new interpreter room for conference {Conference}", conference.Id);
            var roomId = await CreateInterpreterRoom(conference.Id, roomType);
            var ingestUrl = $"{conference.IngestUrl}/{roomId}";
            var vmr = await CreateVmr(conference, roomId, ingestUrl, roomType, existingRooms);
            await UpdateRoomConnectionDetails(conference, roomId, vmr, ingestUrl);
            _logger.LogDebug("Updated room {Room} for conference {Conference} with joining details", roomId,
                conference.Id);

            return await GetUpdatedRoom(conference, roomId, roomType);
        }

        private async Task<long> CreateInterpreterRoom(Guid conferenceId, VirtualCourtRoomType type)
        {
            var createRoomCommand = new CreateInterpreterRoomCommand(conferenceId, type);
            await _commandHandler.Handle(createRoomCommand);
            return createRoomCommand.NewRoomId;
        }

        private Task<BookedParticipantRoomResponse> CreateVmr(Conference conference, long roomId, string ingestUrl,
            VirtualCourtRoomType roomType, int existingRooms)
        {
            var newRoomParams = new CreateParticipantRoomParams
            {
                Audio_recording_url = ingestUrl,
                Participant_room_id = roomId.ToString(),
                Participant_type = roomType.ToString(),
                Room_label_prefix = RoomPrefix,
                Room_type = RoomPrefix,
                Display_name = $"{RoomPrefix}{existingRooms + 1}",
                Tile_number = $"T{200+existingRooms + 1}"
            };
            return _kinlyApiClient.CreateParticipantRoomAsync(conference.Id.ToString(), newRoomParams);
        }

        private InterpreterRoom GetInterpreterRoomForParticipant(Guid conferenceId, IReadOnlyCollection<InterpreterRoom> rooms, Participant participant,
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
            var updateCommand = new UpdateInterpreterRoomConnectionDetailsCommand(conference.Id, roomId, vmr.Room_label,
                ingestUrl, vmr.Uris.Pexip_node, vmr.Uris.Participant);
            return _commandHandler.Handle(updateCommand);
        }

        private async Task<InterpreterRoom> GetUpdatedRoom(Conference conference, long roomId, VirtualCourtRoomType type)
        {
            var query = new GetInterpreterRoomsForConferenceQuery(conference.Id);
            var listOfRooms = await _queryHandler.Handle<GetInterpreterRoomsForConferenceQuery, List<InterpreterRoom>>(query);
            return listOfRooms.First(x => x.Id == roomId);
        }
    }
}
