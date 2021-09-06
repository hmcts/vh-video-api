using System.Collections.Generic;
using System.Linq;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using VideoApi.Extensions;

namespace VideoApi.Mappings
{
    public static class ConferenceForHostResponseMapper
    {
        public static ConferenceForHostResponse MapConferenceSummaryToModel(Conference conference)
        {
            return new ConferenceForHostResponse
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
        
        public static IEnumerable<ParticipantInHearingResponse> MapConferenceSummaryToJudgeInHearingResponse(Conference conference)
        {
            var conferenceId = conference.Id;
            
            return conference.Participants
            .Where(x => x is Participant && ((Participant)x).IsJudge())    
            .Select(x => new ParticipantInHearingResponse
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
        public static ParticipantForHostResponse MapParticipantSummaryToModel(ParticipantBase participant)
        {
            var participantForJudgeResponse = new ParticipantForHostResponse
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
