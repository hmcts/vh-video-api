using System.Collections.Generic;
using System.Linq;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Extensions;

namespace VideoApi.Mappings;

public static class ParticipantInHearingResponseMapper
{
    public static IEnumerable<ParticipantInHearingResponse> MapConferenceSummaryToJudgeInHearingResponse(Conference conference)
    {
        var userRoles = new List<UserRole> { UserRole.Judge };
        return MapConferenceSummaryToHostInHearingResponse(conference, userRoles);
    }
    
    public static IEnumerable<ParticipantInHearingResponse> MapConferenceSummaryToHostInHearingResponse(Conference conference)
    {
        var userRoles = new List<UserRole> { UserRole.Judge, UserRole.StaffMember };
        return MapConferenceSummaryToHostInHearingResponse(conference, userRoles);
    }
    
    private static IEnumerable<ParticipantInHearingResponse> MapConferenceSummaryToHostInHearingResponse(Conference conference, List<UserRole> userRoles)
    {
        var conferenceId = conference.Id;
        return conference.Participants.Where(x => x is Participant && userRoles.Contains(x.UserRole))
            .Select(x => new ParticipantInHearingResponse
            {
                Id = x.Id,
                ConferenceId = conferenceId,
                Status = x.State.MapToContractEnum(),
                Username = x.Username,
                CaseGroup = ((Participant)x).CaseTypeGroup,
                UserRole = x.UserRole.MapToContractEnum()
            });
    }
}
