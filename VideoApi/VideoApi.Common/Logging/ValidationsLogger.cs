namespace VideoApi.Common.Logging
{
    using System;
    using Microsoft.Extensions.Logging;

    public static partial class ValidationsLogger
    {
        [LoggerMessage(EventId = 9000, Level = LogLevel.Debug, Message = "Processing request")]
        public static partial void LogProcessingRequest(this ILogger logger);

        [LoggerMessage(EventId = 9001, Level = LogLevel.Warning, Message = "Request Validation Failed: {Errors}")]
        public static partial void LogRequestValidationFailed(this ILogger logger, string errors);
    }
}