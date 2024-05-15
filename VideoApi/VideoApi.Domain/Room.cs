using System;
using System.Collections.Generic;
using System.Linq;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain
{
    public abstract class Room : TrackableEntity<long>
    { 
        public Guid ConferenceId { get; private set; }
        public string Label { get; protected set; }
        public VirtualCourtRoomType Type { get; private set; }
        public RoomStatus Status { get; private set; }
        public virtual List<RoomParticipant> RoomParticipants { get; }
        public virtual List<RoomEndpoint> RoomEndpoints { get; }
        public bool Locked { get; private set; }
        
        protected Room(Guid conferenceId, string label, VirtualCourtRoomType type, bool locked)
        {
            ConferenceId = conferenceId;
            Label = label;
            Type = type;
            Status = RoomStatus.Live;
            RoomParticipants = new List<RoomParticipant>();
            RoomEndpoints = new List<RoomEndpoint>();
            Locked = locked;
        }

        protected Room(Guid conferenceId, VirtualCourtRoomType type, bool locked) : this(conferenceId, null, type, locked)
        {
        }
        
        public void UpdateRoomLock(bool locked)
        {
            Locked = locked;
        }

        private void UpdateStatus()
        {
            if (Status != RoomStatus.Closed && !RoomParticipants.Any() && !RoomEndpoints.Any() && IsCloseableRoom())
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
