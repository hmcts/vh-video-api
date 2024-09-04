using System;

namespace VideoApi.Contract.Requests;

public class GetConferencesByHearingIdsRequest
{
    public Guid[] HearingRefIds { get; set; }
    public bool IncludeClosed { get; set; }
}
