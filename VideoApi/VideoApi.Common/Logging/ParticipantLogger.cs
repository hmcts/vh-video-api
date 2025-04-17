namespace VideoApi.Common.Logging
{
    using System;
    using Microsoft.Extensions.Logging;

    public static partial class ParticipantLogger
    {
        [LoggerMessage(EventId = 5000, Level = LogLevel.Debug, Message = "AddParticipantsToConference")]
        public static partial void LogAddParticipantsToConference(this ILogger logger);

        [LoggerMessage(EventId = 5001, Level = LogLevel.Debug, Message = "UpdateConferenceParticipants")]
        public static partial void LogUpdateConferenceParticipants(this ILogger logger);

        [LoggerMessage(EventId = 5002, Level = LogLevel.Debug, Message = "UpdateParticipantDetails")]
        public static partial void LogUpdateParticipantDetails(this ILogger logger);

        [LoggerMessage(EventId = 5003, Level = LogLevel.Debug, Message = "RemoveParticipantFromConference")]
        public static partial void LogRemoveParticipantFromConference(this ILogger logger);

        [LoggerMessage(EventId = 5004, Level = LogLevel.Debug, Message = "GetTestCallResultForParticipant")]
        public static partial void LogGetTestCallResultForParticipant(this ILogger logger);

        [LoggerMessage(EventId = 5005, Level = LogLevel.Debug, Message = "GetHeartbeatDataForParticipantAsync")]
        public static partial void LogGetHeartbeatDataForParticipant(this ILogger logger);

        [LoggerMessage(EventId = 5006, Level = LogLevel.Debug, Message = "SaveHeartbeatDataForParticipantAsync")]
        public static partial void LogSaveHeartbeatDataForParticipant(this ILogger logger);

        [LoggerMessage(EventId = 5007, Level = LogLevel.Debug, Message = "GetParticipantsByConferenceId")]
        public static partial void LogGetParticipantsByConferenceId(this ILogger logger);

        [LoggerMessage(EventId = 5008, Level = LogLevel.Debug, Message = "AddStaffMemberToConference")]
        public static partial void LogAddStaffMemberToConference(this ILogger logger);

        [LoggerMessage(EventId = 5009, Level = LogLevel.Debug, Message = "GetHostsInHearingsToday")]
        public static partial void LogGetHostsInHearingsToday(this ILogger logger);

        [LoggerMessage(EventId = 5010, Level = LogLevel.Warning, Message = "Unable to find conference")]
        public static partial void LogUnableToFindConference(this ILogger logger);

        [LoggerMessage(EventId = 5011, Level = LogLevel.Warning, Message = "Unable to find participant {ParticipantId}")]
        public static partial void LogUnableToFindParticipant(this ILogger logger, Guid participantId);

        [LoggerMessage(EventId = 5012, Level = LogLevel.Debug, Message = "Saving test call result")]
        public static partial void LogSavingTestCallResult(this ILogger logger);
        
        [LoggerMessage(EventId = 5013, Level = LogLevel.Error, Message = "Unable to find conference")]
        public static partial void LogUnableToFindConferenceError(this ILogger logger, Exception exception);

        [LoggerMessage(EventId = 5014, Level = LogLevel.Error, Message = "Unable to find participant")]
        public static partial void LogUnableToFindParticipantError(this ILogger logger, Exception exception);

        [LoggerMessage(EventId = 5015, Level = LogLevel.Debug, Message = "Unable to find test call result")]
        public static partial void LogUnableToFindTestCallResult(this ILogger logger);

        [LoggerMessage(EventId = 5016, Level = LogLevel.Warning, Message = "AddHeartbeatRequest is null")]
        public static partial void LogAddHeartbeatRequestNull(this ILogger logger);
    }
}