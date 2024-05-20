using System;
using VideoApi.Domain.Enums;

namespace VideoApi.Domain;

public class EndpointParticipant : TrackableEntity<Guid>
{
    public string ParticipantUsername { get; set; }
    public Guid EndpointId { get; set; }
    public virtual Endpoint Endpoint { get; }
    public LinkedParticipantType Type { get; private set; }
    public EndpointParticipant(Endpoint endpoint, string participantUsername, LinkedParticipantType type)
    {
        Id = Guid.NewGuid();
        EndpointId = endpoint.Id;
        Endpoint = endpoint;
        ParticipantUsername = participantUsername;
        Type = type;
    }
    public EndpointParticipant() {}
}
