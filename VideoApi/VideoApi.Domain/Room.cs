using System;
using System.Collections.Generic;
using System.Linq;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain
{
    public abstract class Room(Guid conferenceId, string label, VirtualCourtRoomType type, bool locked)
        : TrackableEntity<long>
    { 
        public Guid ConferenceId { get; private set; } = conferenceId;
        public string Label { get; protected set; } = label;
        public VirtualCourtRoomType Type { get; } = type;
        public RoomStatus Status { get; private set; } = RoomStatus.Live;
        public virtual List<RoomParticipant> RoomParticipants { get; } = [];
        public virtual List<RoomEndpoint> RoomEndpoints { get; } = [];
        public bool Locked { get; private set; } = locked;

        protected Room(Guid conferenceId, VirtualCourtRoomType type, bool locked) : this(conferenceId, null, type, locked)
        {
        }
        
        public void UpdateRoomLock(bool locked)
        {
            Locked = locked;
        }

        private void UpdateStatus()
        {
            if (Status != RoomStatus.Closed && RoomParticipants.Count == 0 && RoomEndpoints.Count == 0 && IsCloseableRoom())
            {
                Status = RoomStatus.Closed;
            }
        }

        private bool IsCloseableRoom()
        {
            return Type != VirtualCourtRoomType.Civilian && Type != VirtualCourtRoomType.Witness &&
                   Type != VirtualCourtRoomType.JudicialShared;
        }

        public void AddParticipant(RoomParticipant participant)
        {
            if (!DoesParticipantExist(participant))
            {
                RoomParticipants.Add(participant);
            }
        }

        public void RemoveParticipant(RoomParticipant participant)
        {
            if (DoesParticipantExist(participant))
            {
                var existingParticipant = RoomParticipants.Single(x => x.ParticipantId == participant.ParticipantId);

                RoomParticipants.Remove(existingParticipant);
                UpdateStatus();
            }
        }

        public void AddEndpoint(RoomEndpoint endpoint)
        {
            if (!DoesEndpointExist(endpoint))
            {
                RoomEndpoints.Add(endpoint);
            }
        }

        public void RemoveEndpoint(RoomEndpoint endpoint)
        {
            if (DoesEndpointExist(endpoint))
            {
                var existingParticipant = RoomEndpoints.Single(x => x.EndpointId == endpoint.EndpointId);

                RoomEndpoints.Remove(existingParticipant);
                UpdateStatus();
            }
        }

        public void CloseRoom()
        {
            if (RoomParticipants.Count > 0)
                throw new InvalidOperationException("Cannot close a room that has participants in it.");
            
            Status = RoomStatus.Closed;
        }
        
        public List<RoomParticipant> GetRoomParticipants()
        {
            return RoomParticipants;
        }

        public List<RoomEndpoint> GetRoomEndpoints()
        {
            return RoomEndpoints;
        }

        public bool DoesParticipantExist(RoomParticipant participant)
        {
            return RoomParticipants.Exists(x => x.ParticipantId == participant.ParticipantId);
        }

        public bool DoesEndpointExist(RoomEndpoint endpoint)
        {
            return RoomEndpoints.Exists(x => x.EndpointId == endpoint.EndpointId);
        }
    }
}
