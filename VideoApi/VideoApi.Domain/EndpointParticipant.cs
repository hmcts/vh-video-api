using System;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain;

public sealed class EndpointParticipant : TrackableEntity<Guid>
{
    public string Participant { get; set; }
    public Guid EndpointId { get; set; }
    public Endpoint Endpoint { get; set; }
    public LinkedParticipantType Type { get; private set; }
    public EndpointParticipant(Endpoint endpoint, string participantUserName, LinkedParticipantType type)
    {
        Id = Guid.NewGuid();
        Endpoint = endpoint;
        Participant = participantUserName;
        Type = type;
    }
    public EndpointParticipant() {}
}
