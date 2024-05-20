using System.Linq;
using VideoApi.Domain.Enums;
using VideoApi.Contract.Requests;

namespace VideoApi.Mappings;

public static class EndpointParticipantRequestToDtoMapper
{
    public static (string, LinkedParticipantType)[] Map(this EndpointParticipantRequest[] request) 
        => request
            .Select(x => (x.ParticipantUsername, (LinkedParticipantType)x.Type))
            .ToArray();
}
