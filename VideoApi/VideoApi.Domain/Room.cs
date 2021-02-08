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
            Locked = locked;
        }

        public Guid ConferenceId { get; private set; }
        public string Label { get; private set; }
        public VirtualCourtRoomType Type { get; private set; }
        public RoomStatus Status { get; private set; }
        public virtual List<RoomParticipant> RoomParticipants { get; }
        public bool Locked { get; private set; }

        public void UpdateRoomLock(bool locked)
        {
            Locked = locked;
        }

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
            UpdateStatus();
        }

        private void UpdateStatus()
        {
            if (Status != RoomStatus.Closed && !RoomParticipants.Any())
            {
                Status = RoomStatus.Closed;
            }
        }

        public List<RoomParticipant> GetRoomParticipants()
        {
            return RoomParticipants;
        }

        public bool DoesParticipantExist(RoomParticipant participant)
        {
            return RoomParticipants.Any(x => x.ParticipantId == participant.ParticipantId);
        }
    }
}
