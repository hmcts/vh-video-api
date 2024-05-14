using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Requests;

public abstract class EndpointParticipantRequest
{
    public string ParticipantUsername { get; set; }
    public LinkedParticipantType Type { get; set; }
}
