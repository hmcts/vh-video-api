using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Requests;

public class EndpointParticipantRequest
{
    public string ParticipantUsername { get; set; }
    public LinkedParticipantType Type { get; set; }
}
