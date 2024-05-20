using System;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain;

public sealed class EndpointParticipant : TrackableEntity<Guid>
{
    public string ParticipantUsername { get; set; }
    public Guid EndpointId { get; set; }
    public Endpoint Endpoint { get; set; }
    public LinkedParticipantType Type { get; private set; }
    public EndpointParticipant(Endpoint endpoint, string participantUsername, LinkedParticipantType type)
    {
        Id = Guid.NewGuid();
        Endpoint = endpoint;
        ParticipantUsername = participantUsername;
        Type = type;
    }
    public EndpointParticipant() {}
}
