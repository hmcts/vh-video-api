namespace VideoApi.Common.Logging
{
    using System;
    using Microsoft.Extensions.Logging;

    public static partial class InstantMessageLogger
    {
        [LoggerMessage(EventId = 4000, Level = LogLevel.Debug, Message = "Saving instant message")]
        public static partial void LogSavingInstantMessage(this ILogger logger);

        [LoggerMessage(EventId = 4001, Level = LogLevel.Error, Message = "Unable to add instant messages")]
        public static partial void LogUnableToAddInstantMessages(this ILogger logger, Exception exception);

        [LoggerMessage(EventId = 4002, Level = LogLevel.Debug, Message = "RemoveInstantMessagesForConference")]
        public static partial void LogRemovingInstantMessagesForConference(this ILogger logger);

        [LoggerMessage(EventId = 4003, Level = LogLevel.Debug, Message = "InstantMessage deleted")]
        public static partial void LogInstantMessageDeleted(this ILogger logger);

        [LoggerMessage(EventId = 4004, Level = LogLevel.Error, Message = "Unable to remove instant messages")]
        public static partial void LogUnableToRemoveInstantMessages(this ILogger logger, Exception exception);

        [LoggerMessage(EventId = 4005, Level = LogLevel.Debug, Message = "GetClosedConferencesWithInstantMessages")]
        public static partial void LogGettingClosedConferencesWithInstantMessages(this ILogger logger);

        [LoggerMessage(EventId = 4006, Level = LogLevel.Debug, Message = "No closed conferences with instant messages found.")]
        public static partial void LogNoClosedConferencesWithInstantMessagesFound(this ILogger logger);

        [LoggerMessage(EventId = 4007, Level = LogLevel.Debug, Message = "Retrieving instant message history for conference {ConferenceId}")]
        public static partial void LogRetrievingInstantMessageHistory(this ILogger logger, Guid conferenceId);

        [LoggerMessage(EventId = 4008, Level = LogLevel.Debug, Message = "Retrieving instant message history for participant {ParticipantUsername} in conference {ConferenceId}")]
        public static partial void LogRetrievingInstantMessageHistoryForParticipant(this ILogger logger, Guid conferenceId, string participantUsername);

        [LoggerMessage(EventId = 4009, Level = LogLevel.Error, Message = "Unable to retrieve instant message history for conference {ConferenceId}")]
        public static partial void LogUnableToRetrieveInstantMessageHistory(this ILogger logger, Guid conferenceId, Exception exception);

        [LoggerMessage(EventId = 4010, Level = LogLevel.Error, Message = "Unable to retrieve instant message history for participant {ParticipantUsername} in conference {ConferenceId}")]
        public static partial void LogUnableToRetrieveInstantMessageHistoryForParticipant(this ILogger logger, Guid conferenceId, string participantUsername, Exception exception);

    }
}