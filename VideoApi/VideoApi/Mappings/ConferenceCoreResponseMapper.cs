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
        response.HearingRefId = conference.HearingRefId;
        response.ScheduledDateTime = conference.ScheduledDateTime;
        response.StartedDateTime = conference.ActualStartTime;
        response.ClosedDateTime = conference.ClosedDateTime;
        response.ScheduledDuration = conference.ScheduledDuration;
        response.CurrentStatus = conference.GetCurrentStatus().MapToContractEnum();
        response.IsWaitingRoomOpen = conference.IsConferenceAccessible();
        response.CaseName = conference.CaseName;
        return response;
    }
}
