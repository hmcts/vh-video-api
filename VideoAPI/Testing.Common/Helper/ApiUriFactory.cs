using System;

namespace Testing.Common.Helper
{
    public static class ApiUriFactory
    {
        public static class EventsEndpoints
        {
            public const string Event = "events";
        }

        public static class AudioRecordingEndpoints
        {
            private const string ApiRoot = "conferences";
            public static string GetAudioApplication(Guid hearingId) => $"{ApiRoot}/audioapplications/{hearingId}";
            public static string CreateAudioApplication(Guid hearingId) => $"{ApiRoot}/audioapplications/{hearingId}";
            public static string DeleteAudioApplication(Guid hearingId) => $"{ApiRoot}/audioapplications/{hearingId}";
            public static string GetAudioStream(Guid hearingId) => $"{ApiRoot}/audiostreams/{hearingId}";
            public static string GetAudioMonitoringStream(Guid hearingId) => $"{ApiRoot}/audiostreams/{hearingId}/monitoring";
            public static string GetAudioRecordingLink(Guid hearingId) => $"{ApiRoot}/audio/{hearingId}";
            public static string GetCvpAudioRecordingsAll(string cloudRoom, string date, string caseReference) => $"{ApiRoot}/audio/cvp/all/{cloudRoom}/{date}/{caseReference}";
            public static string GetCvpAudioRecordingsByCloudRoom(string cloudRoom, string date) => $"{ApiRoot}/audio/cvp/cloudroom/{cloudRoom}/{date}";
            public static string GetCvpAudioRecordingsByDate(string date, string caseReference) => $"{ApiRoot}/audio/cvp/date/{date}/{caseReference}";
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
            public static string GetDistinctJudgeNames() => $"{ApiRoot}/participants/Judge/firstname";
            public static string GetParticipantsByConferenceId(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/participants";

        }

        public static class ConferenceEndpoints
        {
            private const string ApiRoot = "conferences";
            public static string BookNewConference => $"{ApiRoot}";
            public static string GetConferencesTodayForAdmin => $"{ApiRoot}/today/vho";
            public static string GetConferencesTodayForJudge(string username) => $"{ApiRoot}/today/judge?username=${username}";
            public static string GetConferencesTodayForIndividual(string username) => $"{ApiRoot}/today/individual?username=${username}";
            public static string GetExpiredOpenConferences => $"{ApiRoot}/expired";
            public static string GetConferenceDetailsById(Guid conferenceId) => $"{ApiRoot}/{conferenceId}";
            public static string GetConferenceByHearingRefId(Guid hearingRefId) => $"{ApiRoot}/hearings/{hearingRefId}";
            public static string RemoveConference(Guid conferenceId) => $"{ApiRoot}/{conferenceId}";
            public static string UpdateConference => $"{ApiRoot}";
            public static string CloseConference(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/close";
            public static string GetJudgesInHearingsToday() => $"{ApiRoot}/today/judgesinhearings";
            public static string GetExpiredAudiorecordingConferences => $"{ApiRoot}/audiorecording/expired";
            public static string AnonymiseConferences => $"{ApiRoot}/anonymiseconferences";
            public static string RemoveHeartbeatsForconferences => $"{ApiRoot}/expiredHearbeats";
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
            public static string EndpointConsultationRequest => $"{ApiRoot}/endpoint";
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
            public static string GetInstantMessageHistoryFor(Guid conferenceId, string participantName) => $"{ApiRoot}/{conferenceId}/instantMessages/{participantName}";
            public static string SaveInstantMessage(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/instantmessages";
            public static string RemoveInstantMessagesForConference(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/instantmessages";
            public static string GetClosedConferencesWithInstantMessages => $"{ApiRoot}/expiredIM";
        }
        
        public static class EPEndpoints
        {
            private const string ApiRoot = "conferences";
            public static string GetEndpointsForConference(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/endpoints";
            public static string AddEndpointsToConference(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/endpoints";
            public static string RemoveEndpointsFromConference(Guid conferenceId, string sipAddress) => $"{ApiRoot}/{conferenceId}/endpoints/{sipAddress}";
            public static string UpdateEndpoint(Guid conferenceId, string sipAddress) => $"{ApiRoot}/{conferenceId}/endpoints/{sipAddress}";
        }

        public static class ConferenceManagementEndpoints
        {
            private const string ApiRoot = "conferences";
            public static string StartVideoHearing(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/start";
            public static string PauseVideoHearing(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/pause";
            public static string EndVideoHearing(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/end";
            public static string TransferParticipant(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/transfer";
        }
    }
}
