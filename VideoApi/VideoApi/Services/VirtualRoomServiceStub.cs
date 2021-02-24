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
        private readonly Dictionary<Room, List<Participant>> _rooms = new Dictionary<Room, List<Participant>>();
        public Task<Room> GetOrCreateAnInterpreterVirtualRoom(Conference conference, Participant participant)
        {
            var ids = participant.LinkedParticipants.Select(x => x.Id).ToList();
            ids.Add(participant.Id);

            foreach (var (key, value) in _rooms)
            {
                var roomParticipantIds = value.Select(x => x.Id);
                if (roomParticipantIds.Any(rpid => ids.Contains(rpid)))
                {
                    _rooms[key].Add(participant);
                    return Task.FromResult(key);
                }
            }
            
                
            var room = new Room(Guid.NewGuid(), VirtualCourtRoomType.Civilian, false);
            var label = $"InterpreterRoom{_roomCount + 1}";
            var ingest = $"{conference.IngestUrl}/{_roomCount}";
            var node = "sip.node.com";
            var joinUri = "interpreter__waiting_room";
            room.UpdateRoomDetails(label, ingest, node, joinUri);
            _roomCount++;
            _rooms.Add(room, new List<Participant>{participant});
            return Task.FromResult(room);
        }
    }
}
