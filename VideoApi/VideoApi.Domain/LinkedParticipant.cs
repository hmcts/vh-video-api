using System;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain
{
    public sealed class LinkedParticipant : TrackableEntity<Guid>
    {
        public Guid ParticipantId { get; private set; }
        public ParticipantBase Participant { get; private set; }
        public Guid LinkedId { get; private set; }
        public ParticipantBase Linked { get; private set; }
        public LinkedParticipantType Type { get; private set; }

        public LinkedParticipant(Guid participantId, Guid linkedId, LinkedParticipantType type)
        {
            Id = Guid.NewGuid();
            ParticipantId = participantId;
            LinkedId = linkedId;
            Type = type;
        }

        public LinkedParticipant(ParticipantBase participant, Guid linkedId, LinkedParticipantType type)
        {
            Id = Guid.NewGuid();
            ParticipantId = participant.Id;
            Participant = participant;
            LinkedId = linkedId;
            Type = type;
        }
    }
}
