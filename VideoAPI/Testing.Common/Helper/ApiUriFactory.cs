using System;

namespace Testing.Common.Helper
{
    public class ApiUriFactory
    {
        public EventsEndpoints EventsEndpoints { get; }
        public ParticipantsEndpoints ParticipantsEndpoints { get; }
        public ConferenceEndpoints ConferenceEndpoints { get; }
        public HealthCheckEndpoints HealthCheckEndpoints { get; }
        public ConsultationEndpoints ConsultationEndpoints { get; }
        public TaskEndpoints TaskEndpoints { get; }
        public InstantMessageEndpoints InstantMessageEndpoints { get; }

        public ApiUriFactory()
        {
            ParticipantsEndpoints = new ParticipantsEndpoints();
            ConferenceEndpoints = new ConferenceEndpoints();
            EventsEndpoints = new EventsEndpoints();
            HealthCheckEndpoints = new HealthCheckEndpoints();
            ConsultationEndpoints = new ConsultationEndpoints();
            TaskEndpoints = new TaskEndpoints();
            InstantMessageEndpoints = new InstantMessageEndpoints();
        }
    }
    
    public class EventsEndpoints
    {
        public string Event => "events";
    }
    
    public class ParticipantsEndpoints
    {
        private string ApiRoot => "conferences";

        public string AddParticipantsToConference(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/participants";
        
        public string RemoveParticipantFromConference(Guid conferenceId, Guid participantId) =>
            $"{ApiRoot}/{conferenceId}/participants/{participantId}";
        public string UpdateParticipantFromConference(Guid conferenceId, Guid participantId) =>
            $"{ApiRoot}/{conferenceId}/participants/{participantId}";

        public string GetTestCallResultForParticipant(Guid conferenceId, Guid participantId) =>
            $"{ApiRoot}/{conferenceId}/participants/{participantId}/selftestresult";

        public string GetIndependentTestCallResultForParticipant => $"{ApiRoot}/independentselftestresult";

        public string UpdateParticipantSelfTestScore(Guid conferenceId, Guid participantId) => 
            $"{ApiRoot}/{conferenceId}/participants/{participantId}/updatescore";
    }
    
    public class ConferenceEndpoints
    {
        private string ApiRoot => "conferences";
        public string BookNewConference => $"{ApiRoot}";
        public string GetConferenceDetailsByUsername(string username) => $"{ApiRoot}/?username={username}";
        public string GetConferencesToday => $"{ApiRoot}/today";
        public string GetExpiredOpenConferences => $"{ApiRoot}/expired";
        public string GetConferenceDetailsById(Guid conferenceId) => $"{ApiRoot}/{conferenceId}";
        public string GetConferenceByHearingRefId(Guid hearingRefId) => $"{ApiRoot}/hearings/{hearingRefId}";
        public string RemoveConference(Guid conferenceId) => $"{ApiRoot}/{conferenceId}";
        public string UpdateConference => $"{ApiRoot}";
        public string CloseConference(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/close";
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
        public string LeaveConsultationRequest =>   $"{ApiRoot}/leave";
        public string RespondToAdminConsultationRequest =>   $"{ApiRoot}/vhofficer/respond";
        
    }

    public class TaskEndpoints
    {
        private string ApiRoot => "conferences";

        public string GetTasks(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/tasks";
        public string UpdateTaskStatus(Guid conferenceId, long taskId) => $"{ApiRoot}/{conferenceId}/tasks/{taskId}";
    }

    public class InstantMessageEndpoints
    {
        private string ApiRoot => "conferences";
        public string GetInstantMessageHistory(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/instantmessages";
        public string SaveInstantMessage(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/instantmessages";
        public string RemoveInstantMessagesForConference(Guid conferenceId) =>
           $"{ApiRoot}/{conferenceId}/instantmessages";
    }
}
