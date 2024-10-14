using System.Collections.Generic;
using System.Linq;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Extensions;

namespace VideoApi.Mappings;

public static class ConferenceCoreResponseMapper
{
    public static ConferenceCoreResponse Map(Conference conference)
    {
        var response = new ConferenceCoreResponse();
        response.Id = conference.Id;
        response.HearingId = conference.HearingRefId;
        response.ScheduledDateTime = conference.ScheduledDateTime;
        response.StartedDateTime = conference.ActualStartTime;
        response.ClosedDateTime = conference.ClosedDateTime;
        response.ScheduledDuration = conference.ScheduledDuration;
        response.CurrentStatus = conference.GetCurrentStatus().MapToContractEnum();
        response.IsWaitingRoomOpen = conference.IsConferenceAccessible();
        response.Participants = MapParticipants(conference.Participants);
        response.CaseName = conference.CaseName;
        response.ConferenceRoomType = (Contract.Enums.ConferenceRoomType)conference.ConferenceRoomType;
        return response;
    }
    
    private static List<ParticipantCoreResponse> MapParticipants(IList<ParticipantBase> conferenceParticipants)
    {
        return conferenceParticipants.Select(p => 
            new ParticipantCoreResponse
            {
                Id = p.Id,
                RefId = p.ParticipantRefId,
                UserRole = p.UserRole.MapToContractEnum(),
                DisplayName = p.DisplayName
            }).ToList();
    }
}
