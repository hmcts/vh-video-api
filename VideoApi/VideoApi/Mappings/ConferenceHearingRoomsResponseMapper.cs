using System;
using System.Collections.Generic;
using System.Linq;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace VideoApi.Mappings
{
    public static class ConferenceHearingRoomsResponseMapper
    {
        public static IList<ConferenceHearingRoomsResponse> Map(IList<Conference> conferences, DateTime timeStamp)
        {
            return conferences.SelectMany(x => x.ConferenceStatuses.Where(s=>s.TimeStamp.Date == timeStamp.Date).Where(s=>s.ConferenceState == Domain.Enums.ConferenceState.InSession), (conference, conferenceStatus) =>
                new ConferenceHearingRoomsResponse
                {
                    HearingId = conference.HearingRefId.ToString(),
                    TimeStamp = conferenceStatus.TimeStamp.ToString("O"),
                    ConferenceState = ConferenceState.InSession
                }).ToList();
        }
    }
}
