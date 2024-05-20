using VideoApi.Contract.Enums;

namespace VideoApi.Contract.Responses;

public class EndpointParticipantResponse
{
    public string ParticipantUsername { get; set; }
    public LinkedParticipantType Type { get; set; }
}
