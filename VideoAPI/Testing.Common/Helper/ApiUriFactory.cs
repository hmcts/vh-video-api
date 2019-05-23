using System;

namespace Testing.Common.Helper
{
    public class ApiUriFactory
    {
        public CallbackEndpoints CallbackEndpoints { get; }
        public ParticipantsEndpoints ParticipantsEndpoints { get; }
        public ConferenceEndpoints ConferenceEndpoints { get; }
        public HealthCheckEndpoints HealthCheckEndpoints { get; }
        public ConsultationEndpoints ConsultationEndpoints { get; }
        public TaskEndpoints TaskEndpoints { get; }

        public ApiUriFactory()
        {
            ParticipantsEndpoints = new ParticipantsEndpoints();
            ConferenceEndpoints = new ConferenceEndpoints();
            CallbackEndpoints = new CallbackEndpoints();
            HealthCheckEndpoints = new HealthCheckEndpoints();
            ConsultationEndpoints = new ConsultationEndpoints();
            TaskEndpoints = new TaskEndpoints();
        }
    }
    
    public class CallbackEndpoints
    {
        private string ApiRoot => "callback";
        public string Event => $"{ApiRoot}/conference";
    }
    
    public class ParticipantsEndpoints
    {
        private string ApiRoot => "conferences";

        public string AddParticipantsToConference(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/participants";
        
        public string RemoveParticipantFromConference(Guid conferenceId, Guid participantId) =>
            $"{ApiRoot}/{conferenceId}/participants/{participantId}";

        public string GetTestCallResultForParticipant(Guid conferenceId, Guid participantId) =>
            $"{ApiRoot}/{conferenceId}/participants/{participantId}/selftestresult";
    }
    
    public class ConferenceEndpoints
    {
        private string ApiRoot => "conferences";
        public string BookNewConference => $"{ApiRoot}";
        public string GetConferenceDetailsByUsername(string username) => $"{ApiRoot}/?username={username}";           
        public string GetConferenceDetailsById(Guid conferenceId) => $"{ApiRoot}/{conferenceId}";
        public string GetConferenceByHearingRefId(Guid hearingRefId) => $"{ApiRoot}/hearings/{hearingRefId}";
        public string RemoveConference(Guid conferenceId) => $"{ApiRoot}/{conferenceId}";
    }

    public class HealthCheckEndpoints
    {
        private string ApiRoot => "/healthcheck";

        public string CheckServiceHealth()
        {
            return $"{ApiRoot}/health";
        }
    }

    public class ConsultationEndpoints
    {
        private string ApiRoot => "consultations";

        public string HandleConsultationRequest =>   $"{ApiRoot}";
        
    }

    public class TaskEndpoints
    {
        private string ApiRoot => "conferences";

        public string GetTasks(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/tasks";
        public string UpdateTaskStatus(Guid conferenceId, long taskId) => $"{ApiRoot}/{conferenceId}/tasks/{taskId}";
    }
}