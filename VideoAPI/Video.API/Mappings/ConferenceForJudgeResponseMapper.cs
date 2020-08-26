using System.Collections.Generic;
using System.Linq;
using VideoApi.Contract.Responses;
using VideoApi.Domain;

namespace Video.API.Mappings
{
    public static class ConferenceForJudgeResponseMapper
    {
        public static ConferenceForJudgeResponse MapConferenceSummaryToModel(Conference conference)
        {
            return new ConferenceForJudgeResponse
            {
                Id = conference.Id,
                Status = conference.State,
                CaseName = conference.CaseName,
                CaseNumber = conference.CaseNumber,
                CaseType = conference.CaseType,
                ScheduledDuration = conference.ScheduledDuration,
                ScheduledDateTime = conference.ScheduledDateTime,
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
                Status = x.State,
                Username = x.Username,
                CaseGroup = x.CaseTypeGroup,
                UserRole = x.UserRole
            });
        }
    }
    
    public static class ParticipantForJudgeResponseMapper
    {
        public static ParticipantForJudgeResponse MapParticipantSummaryToModel(Participant participant)
        {
            return new ParticipantForJudgeResponse
            {
                Role = participant.UserRole,
                DisplayName = participant.DisplayName,
                Representee = participant.Representee,
                CaseTypeGroup = participant.CaseTypeGroup
            };
        }
    }
}
