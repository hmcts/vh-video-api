using System.Collections.Generic;
using System.Linq;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Extensions;

namespace VideoApi.Mappings;

public static class HostInHearingResponseMapper
{
    public static IEnumerable<ParticipantInHearingResponse> Map(Conference conference)
    {
        var userRoles = new List<UserRole> { UserRole.Judge, UserRole.StaffMember };
        var conferenceId = conference.Id;
        
        return conference.Participants?.Where(x => x is Participant && userRoles.Contains(x.UserRole))
            .Select(x => new ParticipantInHearingResponse
            {
                Id = x.Id,
                ConferenceId = conferenceId,
                Status = x.State.MapToContractEnum(),
                Username = x.Username,
                UserRole = x.UserRole.MapToContractEnum()
            });
    }
}
