namespace VideoApi.Common.Logging
{
    using System;
    using Microsoft.Extensions.Logging;

    public static partial class EndpointLogger
    {
        [LoggerMessage(EventId = 2000, Level = LogLevel.Debug, Message = "Retrieving endpoints for conference {ConferenceId}")]
        public static partial void LogRetrievingEndpointsForConference(this ILogger logger, Guid conferenceId);

        [LoggerMessage(EventId = 2001, Level = LogLevel.Debug, Message = "Attempting to add endpoint {DisplayName} to conference")]
        public static partial void LogAttemptingToAddEndpoint(this ILogger logger, string displayName);

        [LoggerMessage(EventId = 2002, Level = LogLevel.Debug, Message = "Successfully added endpoint {DisplayName} to conference")]
        public static partial void LogSuccessfullyAddedEndpoint(this ILogger logger, string displayName);

        [LoggerMessage(EventId = 2003, Level = LogLevel.Debug, Message = "Attempting to remove endpoint {SipAddress} from conference")]
        public static partial void LogAttemptingToRemoveEndpoint(this ILogger logger, string sipAddress);

        [LoggerMessage(EventId = 2004, Level = LogLevel.Debug, Message = "Successfully removed endpoint {SipAddress} from conference")]
        public static partial void LogSuccessfullyRemovedEndpoint(this ILogger logger, string sipAddress);

        [LoggerMessage(EventId = 2005, Level = LogLevel.Debug, Message = "Attempting to update endpoint {SipAddress} with display name {DisplayName}")]
        public static partial void LogAttemptingToUpdateEndpoint(this ILogger logger, string sipAddress, string displayName);

        [LoggerMessage(EventId = 2006, Level = LogLevel.Debug, Message = "Successfully updated endpoint {SipAddress} with display name {DisplayName}")]
        public static partial void LogSuccessfullyUpdatedEndpoint(this ILogger logger, string sipAddress, string displayName);
    }
}