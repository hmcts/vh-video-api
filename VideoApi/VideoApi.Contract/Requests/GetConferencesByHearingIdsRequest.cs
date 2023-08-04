using System;

namespace VideoApi.Contract.Requests;

public class GetConferencesByHearingIdsRequest
{
    public Guid[] HearingRefIds { get; set; }
}
