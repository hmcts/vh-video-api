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
        private readonly Dictionary<ParticipantRoom, List<ParticipantBase>> _rooms = new Dictionary<ParticipantRoom, List<ParticipantBase>>();
        public Task<ParticipantRoom> GetOrCreateAnInterpreterVirtualRoom(Conference conference, ParticipantBase participant)
        {
            var label = $"Interpreter{_roomCount + 1}";
            var joinUri = "interpreter__waiting_room_test_stub";
            return CreateRoom(conference, participant, VirtualCourtRoomType.Witness, label, joinUri);
        }

        public Task<ParticipantRoom> GetOrCreateAWitnessVirtualRoom(Conference conference, ParticipantBase participant)
        {
            var label = $"Witness{_roomCount + 1}";
            var joinUri = "witness__waiting_room_test_stub";
            return CreateRoom(conference, participant, VirtualCourtRoomType.Witness, label, joinUri);
        }

        public Task<ParticipantRoom> GetOrCreateAJudicialVirtualRoom(Conference conference, ParticipantBase participant)
        {
            var node = "sip.node.com";
            var joinUri = "panelmember__waiting_room_test_stub";
            var room = new ParticipantRoom(conference.Id, VirtualCourtRoomType.JudicialShared);
            room.UpdateConnectionDetails("PanelMemberRoom1", null, node, joinUri);
            return Task.FromResult(room);
        }

        private Task<ParticipantRoom> CreateRoom(Conference conference, ParticipantBase participant, VirtualCourtRoomType type, string label, string joinUri)
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
            var room = new ParticipantRoom(Guid.NewGuid(), type);
            room.UpdateConnectionDetails(label, ingest, node, joinUri);
            
            _roomCount++;
            _rooms.Add(room, new List<ParticipantBase> {participant});
            return Task.FromResult(room);
        }
    }
}
