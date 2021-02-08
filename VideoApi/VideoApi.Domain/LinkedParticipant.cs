using System;
using VideoApi.Domain.Ddd;

namespace VideoApi.Domain
{
    public sealed class LinkedParticipant  : Entity<Guid>
        {
            public Guid ParticipantId { get; private set; }
            public Participant Participant { get; private set; }
            public Guid LinkedId { get; private set; }
            public Participant Linked { get; private set; }
            public LinkedParticipantType Type { get; private set; }

            public LinkedParticipant(Guid participantId, Guid linkedId, LinkedParticipantType type)
            {
                Id = Guid.NewGuid();
                ParticipantId = participantId;
                LinkedId = linkedId;
                Type = type;
            }
        }
    }
