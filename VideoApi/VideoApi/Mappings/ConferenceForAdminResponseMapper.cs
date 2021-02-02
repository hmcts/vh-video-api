using System.Linq;
using VideoApi.Common.Security.Kinly;
using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace VideoApi.Mappings
{
    public static class ConferenceForAdminResponseMapper
    {
        public static ConferenceForAdminResponse MapConferenceToSummaryResponse(Conference conference,
            KinlyConfiguration configuration)
        {
            var phoneNumber = configuration.ConferencePhoneNumber;
            var participants = conference.GetParticipants()
                .Select(ParticipantToSummaryResponseMapper.MapParticipantToSummary)
                .ToList();

            return new ConferenceForAdminResponse
            {
                Id = conference.Id,
                CaseType = conference.CaseType,
                CaseNumber = conference.CaseNumber,
                CaseName = conference.CaseName,
                ScheduledDateTime = conference.ScheduledDateTime,
                StartedDateTime = conference.ActualStartTime,
                ClosedDateTime = conference.ClosedDateTime,
                ScheduledDuration = conference.ScheduledDuration,
                Status = conference.GetCurrentStatus(),
                Participants = participants,
                HearingRefId = conference.HearingRefId,
                HearingVenueName = conference.HearingVenueName,
                TelephoneConferenceId = conference.MeetingRoom.TelephoneConferenceId,
                TelephoneConferenceNumber = phoneNumber
            };
        }
    }
}
