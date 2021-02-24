using System;
using System.Collections.Generic;
using System.Linq;
using VideoApi.Domain.Ddd;
using VideoApi.Domain.Enums;
using VideoApi.Domain.Validations;

namespace VideoApi.Domain
{
    public class Room : Entity<long>
    {
        public Room(Guid conferenceId, string label, VirtualCourtRoomType type, bool locked)
        {
            ConferenceId = conferenceId;
            Label = label;
            Type = type;
            Status = RoomStatus.Live;
            RoomParticipants = new List<RoomParticipant>();
            RoomEndpoints = new List<RoomEndpoint>();
            Locked = locked;
        }

        public Room(Guid conferenceId, VirtualCourtRoomType type, bool locked) : this(conferenceId, null, type, locked)
        {
        }

        public Guid ConferenceId { get; private set; }
        public string Label { get; private set; }
        public VirtualCourtRoomType Type { get; private set; }
        public RoomStatus Status { get; private set; }
        public virtual List<RoomParticipant> RoomParticipants { get; }
        public virtual List<RoomEndpoint> RoomEndpoints { get; }
        public bool Locked { get; private set; }
        
        public string IngestUrl { get; private set; }
        public string PexipNode { get; private set; }
        public string ParticipantUri { get; private set; }

        public void UpdateRoomLock(bool locked)
        {
            Locked = locked;
        }

        public void AddParticipant(RoomParticipant participant)
        {
            if (DoesParticipantExist(participant))
            {
                throw new DomainRuleException(nameof(participant), "Participant already exists in room");
            }

            RoomParticipants.Add(participant);
        }

        public void RemoveParticipant(RoomParticipant participant)
        {
            if (!DoesParticipantExist(participant))
            {
                throw new DomainRuleException(nameof(participant), "Participant does not exist in room");
            }

            var existingParticipant = RoomParticipants.Single(x => x.ParticipantId == participant.ParticipantId);

            RoomParticipants.Remove(existingParticipant);
            UpdateStatus();
        }

        public void AddEndpoint(RoomEndpoint endpoint)
        {
            if (DoesEndpointExist(endpoint))
            {
                throw new DomainRuleException(nameof(endpoint), "Endpoint already exists in room");
            }

            RoomEndpoints.Add(endpoint);
        }

        public void RemoveEndpoint(RoomEndpoint endpoint)
        {
            if (!DoesEndpointExist(endpoint))
            {
                throw new DomainRuleException(nameof(endpoint), "Endpoint does not exist in room");
            }

            var existingParticipant = RoomEndpoints.Single(x => x.EndpointId == endpoint.EndpointId);

            RoomEndpoints.Remove(existingParticipant);
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            if (Status != RoomStatus.Closed && !RoomParticipants.Any() && !RoomEndpoints.Any())
            {
                Status = RoomStatus.Closed;
            }
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
            return RoomParticipants.Any(x => x.ParticipantId == participant.ParticipantId);
        }

        public bool DoesEndpointExist(RoomEndpoint endpoint)
        {
            return RoomEndpoints.Any(x => x.EndpointId == endpoint.EndpointId);
        }

        public void AddRoomConnectionDetails(string label, string ingestUrl, string pexipNode, string participantUri)
        {
            Label = label;
            IngestUrl = ingestUrl;
            PexipNode = pexipNode;
            ParticipantUri = participantUri;
        }
    }
}
