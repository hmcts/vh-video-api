using System;
using System.Collections.Generic;
using VideoApi.Contract.Requests;
using VideoApi.Contract.Responses;
using VideoApi.Domain;
using Alert = VideoApi.Domain.Task;

namespace Testing.Common.Configuration
{
    public class Test
    {
        public Test()
        {
            CvpFileNamesOnStorage = new List<string>();    
        }
        
        public string CaseName { get; set; }
        public List<Conference> ClosedConferences { get; set; }
        public List<Conference> ClosedConferencesWithMessages { get; set; }
        public Conference Conference { get; set; }
        public List<Alert> Alerts { get; set; }
        public List<Guid> ConferenceIds { get; set; }
        public ConferenceDetailsResponse ConferenceResponse { get; set; }
        public List<ConferenceDetailsResponse> ConferenceDetailsResponses { get; set; }
        public List<ConferenceForAdminResponse> ConferenceResponses { get; set; }
        public List<ConferenceForJudgeResponse> ConferenceJudgeResponses { get; set; }
        public List<ConferenceForIndividualResponse> ConferenceIndividualResponses { get; set; }
        public Guid TomorrowsConference { get; set; }
        public List<Conference> Conferences { get; set; }
        public AddHeartbeatRequest HeartbeatData { get; set; }
        public AddInstantMessageRequest Message { get; set; }
        public Guid ParticipantId { get; set; }
        public long TaskId { get; set; }
        public List<Conference> TodaysConferences { get; set; }
        public UpdateTaskRequest UpdateTaskRequest { get; set; }
        public Conference YesterdayClosedConference { get; set; }
        public List<JudgeInHearingResponse> JudgeInHearings { get; set; }
        public List<string> CvpFileNamesOnStorage { get; set; }
    }
}
