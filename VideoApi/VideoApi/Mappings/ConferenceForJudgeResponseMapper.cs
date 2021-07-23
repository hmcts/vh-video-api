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
            .Where(x => x is Participant && ((Participant)x).IsJudge())    
            .Select(x => new JudgeInHearingResponse
            {
                Id = x.Id,
                ConferenceId = conferenceId,
                Status = x.State.MapToContractEnum(),
                Username = x.Username,
                CaseGroup = ((Participant)x).CaseTypeGroup,
                UserRole = x.UserRole.MapToContractEnum()
            });
        }
    }
    
    public static class ParticipantForJudgeResponseMapper
    {
        public static ParticipantForJudgeResponse MapParticipantSummaryToModel(ParticipantBase participant)
        {
            var participantForJudgeResponse = new ParticipantForJudgeResponse
            {
                Id = participant.Id,
                Role = participant.UserRole.MapToContractEnum(),
                DisplayName = participant.DisplayName,
            };

            if (participant is Participant participantCasted)
            {
                participantForJudgeResponse.Representee = participantCasted.Representee;
                participantForJudgeResponse.CaseTypeGroup = participantCasted.CaseTypeGroup;
                participantForJudgeResponse.HearingRole = participantCasted.HearingRole;
            }

            return participantForJudgeResponse;
        }
    }
}
