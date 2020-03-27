using System;

namespace Testing.Common.Helper
{
    public static class ApiUriFactory
    {
        public static class EventsEndpoints
        {
            public const string Event = "events";
        }

        public static class ParticipantsEndpoints
        {
            private const string ApiRoot = "conferences";
            public static string AddParticipantsToConference(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/participants";
            public static string RemoveParticipantFromConference(Guid conferenceId, Guid participantId) => $"{ApiRoot}/{conferenceId}/participants/{participantId}";
            public static string UpdateParticipantFromConference(Guid conferenceId, Guid participantId) => $"{ApiRoot}/{conferenceId}/participants/{participantId}";
            public static string GetTestCallResultForParticipant(Guid conferenceId, Guid participantId) => $"{ApiRoot}/{conferenceId}/participants/{participantId}/selftestresult";
            public static string GetIndependentTestCallResultForParticipant => $"{ApiRoot}/independentselftestresult";
            public static string UpdateParticipantSelfTestScore(Guid conferenceId, Guid participantId) => $"{ApiRoot}/{conferenceId}/participants/{participantId}/updatescore";
            public static string GetHeartbeats(Guid conferenceId, Guid participantId) => $"{ApiRoot}/{conferenceId}/participant/{participantId}/heartbeatrecent";
            public static string SetHeartbeats(Guid conferenceId, Guid participantId) => $"{ApiRoot}/{conferenceId}/participant/{participantId}/heartbeat";
        }

        public static class ConferenceEndpoints
        {
            private const string ApiRoot = "conferences";
            public static string BookNewConference => $"{ApiRoot}";
            public static string GetConferencesToday => $"{ApiRoot}/today";
            public static string GetConferencesTodayForJudge(string username) => $"{ApiRoot}/today/judge?username=${username}";
            public static string GetConferencesTodayForIndividual(string username) => $"{ApiRoot}/today/individual?username=${username}";
            public static string GetExpiredOpenConferences => $"{ApiRoot}/expired";
            public static string GetConferenceDetailsById(Guid conferenceId) => $"{ApiRoot}/{conferenceId}";
            public static string GetConferenceByHearingRefId(Guid hearingRefId) => $"{ApiRoot}/hearings/{hearingRefId}";
            public static string RemoveConference(Guid conferenceId) => $"{ApiRoot}/{conferenceId}";
            public static string UpdateConference => $"{ApiRoot}";
            public static string CloseConference(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/close";
        }

        public static class HealthCheckEndpoints
        {
            private const string ApiRoot = "/healthcheck";
            public static string CheckServiceHealth => $"{ApiRoot}/health";
        }

        public static class ConsultationEndpoints
        {
            private const string ApiRoot = "consultations";
            public static string HandleConsultationRequest => $"{ApiRoot}";
            public static string LeaveConsultationRequest => $"{ApiRoot}/leave";
            public static string RespondToAdminConsultationRequest => $"{ApiRoot}/vhofficer/respond";
        }

        public static class SelfTestEndpoints
        {
            private const string ApiRoot = "selftest";
            public static string SelfTest => $"{ApiRoot}";
        }

        public static class TaskEndpoints
        {
            private const string ApiRoot = "conferences";
            public static string GetTasks(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/tasks";
            public static string UpdateTaskStatus(Guid conferenceId, long taskId) => $"{ApiRoot}/{conferenceId}/tasks/{taskId}";
        }

        public static class InstantMessageEndpoints
        {
            private const string ApiRoot = "conferences";
            public static string GetInstantMessageHistory(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/instantmessages";
            public static string SaveInstantMessage(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/instantmessages";
            public static string RemoveInstantMessagesForConference(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/instantmessages";
            public static string GetClosedConferencesWithInstantMessages => $"{ApiRoot}/expiredIM";
        }
    }
}
