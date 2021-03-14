using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Contracts;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services
{
    public class VirtualRoomServiceStub : IVirtualRoomService
    {
        private int _roomCount;
        private readonly Dictionary<InterpreterRoom, List<Participant>> _rooms = new Dictionary<InterpreterRoom, List<Participant>>();
        public Task<InterpreterRoom> GetOrCreateAnInterpreterVirtualRoom(Conference conference, Participant participant)
        {
            var label = $"Interpreter{_roomCount + 1}";
            var joinUri = "interpreter__waiting_room";
            return CreateRoom(conference, participant, VirtualCourtRoomType.Witness, label, joinUri);
        }

        public Task<InterpreterRoom> GetOrCreateAWitnessVirtualRoom(Conference conference, Participant participant)
        {
            var label = $"Witness{_roomCount + 1}";
            var joinUri = "witness__waiting_room";
            return CreateRoom(conference, participant, VirtualCourtRoomType.Witness, label, joinUri);
        }

        private Task<InterpreterRoom> CreateRoom(Conference conference, Participant participant, VirtualCourtRoomType type, string label, string joinUri)
        {
            var ids = participant.LinkedParticipants.Select(x => x.Id).ToList();
            ids.Add(participant.Id);
            
            foreach (var (key, value) in _rooms)
            {
                var roomParticipantIds = value.Select(x => x.Id);
                if (!roomParticipantIds.Any(rpid => ids.Contains(rpid))) continue;
                _rooms[key].Add(participant);
                return Task.FromResult(key);
            }
            var ingest = $"{conference.IngestUrl}/{_roomCount}";
            var node = "sip.node.com";
            var room = new InterpreterRoom(Guid.NewGuid(), type);
            room.UpdateRoomConnectionDetails(label, ingest, node, joinUri);
            
            room.UpdateRoomConnectionDetails(label, ingest, node, joinUri);
            _roomCount++;
            _rooms.Add(room, new List<Participant>{participant});
            return Task.FromResult(room);
        }
    }
}
