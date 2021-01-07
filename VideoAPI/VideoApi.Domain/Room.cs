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
        public Room(Guid conferenceId, string label, VirtualCourtRoomType type)
        {
            ConferenceId = conferenceId;
            Label = label;
            Type = type;
            Status = RoomStatus.Created;
            RoomParticipants = new List<RoomParticipant>();
        }

        public Guid ConferenceId { get; private set; }
        public string Label { get; private set; }
        public VirtualCourtRoomType Type { get; private set; }
        public RoomStatus Status { get; private set; }

        public virtual List<RoomParticipant> RoomParticipants { get; }

        public void AddParticipant(RoomParticipant participant)
        {
            if (DoesParticipantExist(participant))
            {
                throw new DomainRuleException(nameof(participant), "Participant already exists in conference");
            }

            RoomParticipants.Add(participant);
        }

        public void RemoveParticipant(RoomParticipant participant)
        {
            if (!DoesParticipantExist(participant))
            {
                throw new DomainRuleException(nameof(participant), "Participant does not exist in conference");
            }

            var existingParticipant = RoomParticipants.Single(x => x.ParticipantId == participant.ParticipantId);

            RoomParticipants.Remove(existingParticipant);
        }

        public void UpdateStatus(RoomStatus status)
        {
            if (Status == RoomStatus.Closed)
            {
                throw new DomainRuleException("Status", "Could not change status for a closed room");
            }

            Status = status;
        }

        public bool DoesParticipantExist(RoomParticipant participant)
        {
            return RoomParticipants.Any(x => x.ParticipantId == participant.ParticipantId);
        }
    }
}
