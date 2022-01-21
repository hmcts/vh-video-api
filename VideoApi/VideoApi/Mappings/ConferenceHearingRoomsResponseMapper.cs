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
        public static IList<ConferenceHearingRoomsResponse> Map(IList<HearingAudioRoom> audioRecordedConferences, DateTime timeStamp)
        {
            return audioRecordedConferences.Select((audioConference) =>
                new ConferenceHearingRoomsResponse
                {
                    HearingId = audioConference.HearingRefId.ToString(),
                    Label = audioConference.Label,
                    FileNamePrefix = audioConference.FileNamePrefix
                }).ToList();
        }
    }
}
