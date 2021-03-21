using System.Collections.Generic;
using System.Linq;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Extensions;

namespace VideoApi.Mappings
{
    public static class ConferenceForJudgeResponseMapper
    {
        public static ConferenceForJudgeResponse MapConferenceSummaryToModel(Conference conference)
        {
            return new ConferenceForJudgeResponse
            {
                Id = conference.Id,
                Status = conference.GetCurrentStatus().MapToContractEnum(),
                CaseName = conference.CaseName,
                CaseNumber = conference.CaseNumber,
                CaseType = conference.CaseType,
                ScheduledDuration = conference.ScheduledDuration,
                ScheduledDateTime = conference.ScheduledDateTime,
                ClosedDateTime = conference.ClosedDateTime,
                Participants = conference.Participants
                    .Select(ParticipantForJudgeResponseMapper.MapParticipantSummaryToModel).ToList(),
                NumberOfEndpoints = conference.Endpoints.Count
            };
        }
        
        public static IEnumerable<JudgeInHearingResponse> MapConferenceSummaryToJudgeInHearingResponse(Conference conference)
        {
            var conferenceId = conference.Id;
            
            return conference.Participants
            .Where(x => x.IsJudge())    
            .Select(x => new JudgeInHearingResponse
            {
                Id = x.Id,
                ConferenceId = conferenceId,
                Status = x.State.MapToContractEnum(),
                Username = x.Username,
                CaseGroup = x.CaseTypeGroup,
                UserRole = x.UserRole.MapToContractEnum()
            });
        }
    }
    
    public static class ParticipantForJudgeResponseMapper
    {
        public static ParticipantForJudgeResponse MapParticipantSummaryToModel(Participant participant)
        {
            return new ParticipantForJudgeResponse
            {
                Id = participant.Id,
                Role = participant.UserRole.MapToContractEnum(),
                DisplayName = participant.DisplayName,
                Representee = participant.Representee,
                CaseTypeGroup = participant.CaseTypeGroup,
                HearingRole = participant.HearingRole
            };
        }
    }
}
