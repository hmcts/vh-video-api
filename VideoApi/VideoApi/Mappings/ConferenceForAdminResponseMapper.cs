using System.Linq;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Common.Security.Supplier.Kinly;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Extensions;

namespace VideoApi.Mappings
{
    public static class ConferenceForAdminResponseMapper
    {
        public static ConferenceForAdminResponse MapConferenceToSummaryResponse(Conference conference, SupplierConfiguration configuration)
        {
            var phoneNumbers = $"{configuration.ConferencePhoneNumber},{configuration.ConferencePhoneNumberWelsh}";
            var participants = conference.GetParticipants()
                .Select(p => ParticipantResponseMapper.Map(p))
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
                Status = conference.GetCurrentStatus().MapToContractEnum(),
                Participants = participants,
                HearingRefId = conference.HearingRefId,
                HearingVenueName = conference.HearingVenueName,
                TelephoneConferenceId = conference.MeetingRoom.TelephoneConferenceId,
                TelephoneConferenceNumbers = phoneNumbers,
                CreatedDateTime = conference.CreatedDateTime,
                IsWaitingRoomOpen = conference.IsConferenceAccessible()
            };
        }
    }
}
