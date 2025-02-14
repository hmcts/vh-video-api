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
            
            public static string GetAudioStream(Guid hearingId) => $"{ApiRoot}/audiostreams/{hearingId}";
            public static string GetAudioRecordingLink(Guid hearingId) => $"{ApiRoot}/audio/{hearingId}";
        }
        
        public static class ParticipantsEndpoints
        {
            private const string ApiRoot = "conferences";
            
            public static string AddParticipantsToConference(Guid conferenceId) =>
                $"{ApiRoot}/{conferenceId}/participants";
            
            public static string RemoveParticipantFromConference(Guid conferenceId, Guid participantId) =>
                $"{ApiRoot}/{conferenceId}/participants/{participantId}";
            
            public static string UpdateParticipantFromConference(Guid conferenceId, Guid participantId) =>
                $"{ApiRoot}/{conferenceId}/participants/{participantId}";
            
            public static string GetTestCallResultForParticipant(Guid conferenceId, Guid participantId) =>
                $"{ApiRoot}/{conferenceId}/participants/{participantId}/selftestresult";
            
            public static string GetHeartbeats(Guid conferenceId, Guid participantId) =>
                $"{ApiRoot}/{conferenceId}/participant/{participantId}/heartbeatrecent";
            
            public static string SetHeartbeats(Guid conferenceId, Guid participantId) =>
                $"{ApiRoot}/{conferenceId}/participant/{participantId}/heartbeat";
            
            public static string GetDistinctJudgeNames() => $"{ApiRoot}/participants/Judge/firstname";
            
            public static string GetParticipantsByConferenceId(Guid conferenceId) =>
                $"{ApiRoot}/{conferenceId}/participants";
            
            public static string UpdateParticipantUsername(Guid participantId) =>
                $"{ApiRoot}/participants/{participantId}/username";
        }
        
        public static class ConferenceEndpoints
        {
            private const string ApiRoot = "conferences";
            public static string BookNewConference => $"{ApiRoot}";
            public static string GetConferencesToday => $"{ApiRoot}/today";
            public static string GetExpiredOpenConferences => $"{ApiRoot}/expired";
            public static string UpdateConference => $"{ApiRoot}";
            public static string GetExpiredAudiorecordingConferences => $"{ApiRoot}/audiorecording/expired";
            public static string AnonymiseConferences => $"{ApiRoot}/anonymiseconferences";
            public static string RemoveHeartbeatsForconferences => $"{ApiRoot}/expiredHeartbeats";
            public static string GetConferencesByHearingRefIds() => $"{ApiRoot}/hearings";
            public static string GetConferenceDetailsByHearingRefIds() => $"{ApiRoot}/hearings/details";
            
            public static string GetConferencesTodayForIndividual(string username) =>
                $"{ApiRoot}/today/individual?username={username}";
            
            public static string GetConferenceDetailsById(Guid conferenceId) => $"{ApiRoot}/{conferenceId}";
            public static string RemoveConference(Guid conferenceId) => $"{ApiRoot}/{conferenceId}";
            public static string CloseConference(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/close";
        }
        
        public static class ConsultationEndpoints
        {
            private const string ApiRoot = "consultations";
            public static string HandleConsultationRequest => $"{ApiRoot}";
            public static string LeaveConsultationRequest => $"{ApiRoot}/leave";
            public static string EndpointConsultationRequest => $"{ApiRoot}/endpoint";
            public static string StartPrivateConsultationRequest => $"{ApiRoot}/start";
            public static string LeavePrivateConsultationRequest => $"{ApiRoot}/end";
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
            
            public static string UpdateTaskStatus(Guid conferenceId, long taskId) =>
                $"{ApiRoot}/{conferenceId}/tasks/{taskId}";
            
            public static string AddTask(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/task";
        }
        
        public static class InstantMessageEndpoints
        {
            private const string ApiRoot = "conferences";
            public static string GetClosedConferencesWithInstantMessages => $"{ApiRoot}/expiredIM";
            
            public static string GetInstantMessageHistory(Guid conferenceId) =>
                $"{ApiRoot}/{conferenceId}/instantmessages";
            
            public static string GetInstantMessageHistoryFor(Guid conferenceId, string participantName) =>
                $"{ApiRoot}/{conferenceId}/instantMessages/{participantName}";
            
            public static string SaveInstantMessage(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/instantmessages";
            
            public static string RemoveInstantMessagesForConference(Guid conferenceId) =>
                $"{ApiRoot}/{conferenceId}/instantmessages";
        }
        
        public static class EPEndpoints
        {
            private const string ApiRoot = "conferences";
            public static string GetEndpointsForConference(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/endpoints";
            public static string AddEndpointsToConference(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/endpoints";
            
            public static string RemoveEndpointsFromConference(Guid conferenceId, string sipAddress) =>
                $"{ApiRoot}/{conferenceId}/endpoints/{sipAddress}";
            
            public static string UpdateEndpoint(Guid conferenceId, string sipAddress) =>
                $"{ApiRoot}/{conferenceId}/endpoints/{sipAddress}";
        }
        
        public static class ConferenceManagementEndpoints
        {
            private const string ApiRoot = "conferences";
            public static string StartVideoHearing(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/start";
            public static string PauseVideoHearing(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/pause";
            public static string EndVideoHearing(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/end";
            public static string SuspendVideoHearing(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/suspend";
            public static string TransferParticipant(Guid conferenceId) => $"{ApiRoot}/{conferenceId}/transfer";
        }
        
        public static class VirtualRoomEndpoints
        {
            private const string ApiRoot = "conferences";
            
            public static string GetInterpreterRoomForParticipant(Guid conferenceId, Guid participantId) =>
                $"{ApiRoot}/{conferenceId}/rooms/interpreter/{participantId}";
            
            public static string GetWitnessRoomForParticipant(Guid conferenceId, Guid participantId) =>
                $"{ApiRoot}/{conferenceId}/rooms/witness/{participantId}";
            
            public static string GetJudicialRoomForParticipant(Guid conferenceId, Guid participantId) =>
                $"{ApiRoot}/{conferenceId}/rooms/judicial/{participantId}";
        }
    }
}
