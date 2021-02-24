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

namespace VideoApi.Services
{
    public class VirtualRoomService : IVirtualRoomService
    {
        private readonly IKinlyApiClient _kinlyApiClient;
        private readonly ILogger<VirtualRoomService> _logger;
        private readonly ICommandHandler _commandHandler;
        private readonly IQueryHandler _queryHandler;

        public VirtualRoomService(IKinlyApiClient kinlyApiClient, ILogger<VirtualRoomService> logger,
            ICommandHandler commandHandler, IQueryHandler queryHandler)
        {
            _kinlyApiClient = kinlyApiClient;
            _logger = logger;
            _commandHandler = commandHandler;
            _queryHandler = queryHandler;
        }

        public async Task<Room> GetOrCreateAnInterpreterVirtualRoom(Conference conference, Participant participant)
        {
            var conferenceId = conference.Id;

            // get a room
            var roomPrefix = "Interpreter";
            var query = new GetAvailableRoomByRoomTypeQuery(VirtualCourtRoomType.Civilian, conferenceId);
            var listOfRooms = await _queryHandler.Handle<GetAvailableRoomByRoomTypeQuery, List<Room>>(query);
            var interpreterRooms = listOfRooms.Where(x => x.Label.Contains(roomPrefix)).ToList();
            var room = GetRoomForParticipant(conferenceId, interpreterRooms, participant) ??
                       await CreateAnInterpreterRoom(conference, interpreterRooms.Count);

            return room;
        }

        public async Task<Room> CreateAnInterpreterRoom(Conference conference, int existingRooms)
        {
            _logger.LogInformation("Creating a new interpreter room for conference {Conference}", conference.Id);
            var createRoomCommand = new CreateRoomCommand(conference.Id, null, VirtualCourtRoomType.Civilian, false);
            await _commandHandler.Handle(createRoomCommand);
            var roomId = createRoomCommand.NewRoomId;
            _logger.LogInformation("Creating a new civilian room {Room} for conference {Conference}", roomId,
                conference.Id);
            var ingestUrl = $"{conference.IngestUrl}/{roomId}";
            var roomPrefix = "Interpreter";

            var newRoomParams = new CreateParticipantRoomParams
            {
                Audio_recording_url = ingestUrl,
                Participant_room_id = roomId.ToString(),
                Participant_type = VirtualCourtRoomType.Civilian.ToString(),
                Room_label_prefix = roomPrefix,
                Room_type = roomPrefix,
                Display_name = $"{roomPrefix}{existingRooms + 1}",
                Tile_number = $"T{existingRooms + 1}"
            };
            var newVmr = await _kinlyApiClient.CreateParticipantRoomAsync(conference.Id.ToString(), newRoomParams);

            var updateCommand = new UpdateRoomCommand(conference.Id, roomId, newVmr.Display_name, ingestUrl,
                newVmr.Uris.Pexip_node, newVmr.Uris.Participant);
            await _commandHandler.Handle(updateCommand);
            _logger.LogDebug("Updated room {Room} for conference {Conference} with joining details", roomId,
                conference.Id);
            var query = new GetAvailableRoomByRoomTypeQuery(VirtualCourtRoomType.Civilian, conference.Id);
            var listOfRooms = await _queryHandler.Handle<GetAvailableRoomByRoomTypeQuery, List<Room>>(query);

            return listOfRooms.First(x => x.Id == roomId);
        }

        private Room GetRoomForParticipant(Guid conferenceId, IReadOnlyCollection<Room> rooms, Participant participant)
        {
            _logger.LogInformation(
                "Checking for an existing interpreter room for participant {Participant} in conference {Conference}",
                participant.Id, conferenceId);
            var participantIds = participant.LinkedParticipants.Select(lp => lp.LinkedId).ToList();
            participantIds.Add(participant.Id);

            // first check if a room with linked participants for current participant exist
            var matchingRooms = participantIds
                .Select(l => rooms.SingleOrDefault(r => r.DoesParticipantExist(new RoomParticipant(l))))
                .Where(m => m != null)
                .Select(matching => matching)
                .ToList();

            var existingRoom = matchingRooms.FirstOrDefault();
            // return an empty room, if possible
            return existingRoom ?? rooms.FirstOrDefault(x => x.RoomParticipants.Count == 0);

        }
    }
}
