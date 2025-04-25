namespace VideoApi.Common.Logging
{
    using System;
    using Microsoft.Extensions.Logging;

    public static partial class RecordingLogger
    {
        [LoggerMessage(
            EventId = 6000, 
            Level = LogLevel.Information,
            Message = "Getting audio recording link")]
        public static partial void LogInformationGettingAudioLink(this ILogger logger);
        
        [LoggerMessage(
            EventId = 6001, 
            Level = LogLevel.Information,
            Message = "Getting audio recording link for CVP cloud room")]
        public static partial void LogInformationGettingAudioLinkForCvp(this ILogger logger);
        
        [LoggerMessage(
            EventId = 6002, 
            Level = LogLevel.Information,
            Message = "Recording connection failed callback - {ConferenceId}")]
        public static partial void LogInformationRecordingConnectionFailedCallback(this ILogger logger, Guid conferenceId);
        
        [LoggerMessage(
            EventId = 7000, 
            Level = LogLevel.Error,
            Message = "{Message}")]
        public static partial void LogErrorGeneric(this ILogger logger, Exception ex, string message);

        [LoggerMessage(
            EventId = 7001, 
            Level = LogLevel.Error,
            Message = "Not found: {Message}")]
        public static partial void LogErrorNotFound(this ILogger logger, Exception ex, string message);

    }
}