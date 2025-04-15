namespace VideoApi.Common.Logging
{
    using System;
    using Microsoft.Extensions.Logging;

    public static partial class ConferenceLogger
    {
        [LoggerMessage(EventId = 1000, Level = LogLevel.Debug, Message = "BookNewConference")]
        public static partial void LogBookNewConference(this ILogger logger);

        [LoggerMessage(EventId = 1001, Level = LogLevel.Debug, Message = "Conference Created")]
        public static partial void LogConferenceCreatedDebug(this ILogger logger);

        [LoggerMessage(EventId = 1002, Level = LogLevel.Debug, Message = "Room Booked")]
        public static partial void LogRoomBooked(this ILogger logger);

        [LoggerMessage(EventId = 1003, Level = LogLevel.Error, Message = "Could not book and find meeting room for conferenceId: {ConferenceId}")]
        public static partial void LogRoomBookingFailed(this ILogger logger, Guid conferenceId);

        [LoggerMessage(EventId = 1004, Level = LogLevel.Information, Message = "Created conference {ResponseId} for hearing {HearingRefId}")]
        public static partial void LogConferenceCreatedInfo(this ILogger logger, Guid responseId, Guid hearingRefId);

        [LoggerMessage(EventId = 1005, Level = LogLevel.Debug, Message = "UpdateConference")]
        public static partial void LogUpdateConference(this ILogger logger);

        [LoggerMessage(EventId = 1006, Level = LogLevel.Warning, Message = "Unable to find conference with hearing id {HearingRefId}")]
        public static partial void LogConferenceNotFoundWarning(this ILogger logger, Guid hearingRefId);

        [LoggerMessage(EventId = 1007, Level = LogLevel.Debug, Message = "GetConferenceDetailsById {ConferenceId}")]
        public static partial void LogGetConferenceDetailsById(this ILogger logger, Guid conferenceId);

        [LoggerMessage(EventId = 1008, Level = LogLevel.Warning, Message = "Unable to find conference {ConferenceId}")]
        public static partial void LogConferenceNotFound(this ILogger logger, Guid conferenceId);

        [LoggerMessage(EventId = 1009, Level = LogLevel.Debug, Message = "RemoveConference {ConferenceId}")]
        public static partial void LogRemoveConference(this ILogger logger, Guid conferenceId);

        [LoggerMessage(EventId = 1010, Level = LogLevel.Information, Message = "Successfully removed conference {ConferenceId}")]
        public static partial void LogConferenceRemoved(this ILogger logger, Guid conferenceId);

        [LoggerMessage(EventId = 1011, Level = LogLevel.Error, Message = "Unable to find conference {ConferenceId}")]
        public static partial void LogConferenceNotFoundError(this ILogger logger, Guid conferenceId, Exception exception);

        [LoggerMessage(EventId = 1012, Level = LogLevel.Debug, Message = "GetConferencesTodayForIndividualByUsername {Username}")]
        public static partial void LogGetConferencesTodayForIndividual(this ILogger logger, string username);

        [LoggerMessage(EventId = 1013, Level = LogLevel.Warning, Message = "Invalid username {Username}")]
        public static partial void LogInvalidUsername(this ILogger logger, string username);

        [LoggerMessage(EventId = 1014, Level = LogLevel.Debug, Message = "GetExpiredOpenConferences")]
        public static partial void LogGetExpiredOpenConferences(this ILogger logger);

        [LoggerMessage(EventId = 1015, Level = LogLevel.Debug, Message = "GetExpiredAudiorecordingConferences")]
        public static partial void LogGetExpiredAudiorecordingConferences(this ILogger logger);

        [LoggerMessage(EventId = 1016, Level = LogLevel.Debug, Message = "AnonymiseConferencesAndParticipantInformation")]
        public static partial void LogAnonymiseConferences(this ILogger logger);

        [LoggerMessage(EventId = 1017, Level = LogLevel.Information, Message = "Records updated: {RecordsUpdated}")]
        public static partial void LogRecordsUpdated(this ILogger logger, int recordsUpdated);

        [LoggerMessage(EventId = 1018, Level = LogLevel.Warning, Message = "Failed to BookMeetingRoomAsync. Retrying attempt {RetryAttempt}")]
        public static partial void LogRetryBookingMeetingRoom(this ILogger logger, int retryAttempt);

        [LoggerMessage(EventId = 1019, Level = LogLevel.Warning, Message = "Failed to CreateConferenceAsync. Retrying attempt {RetryAttempt}")]
        public static partial void LogRetryCreateConference(this ILogger logger, int retryAttempt);

        [LoggerMessage(EventId = 1020, Level = LogLevel.Debug, Message = "Attempting to start hearing")]
        public static partial void LogStartHearing(this ILogger logger);

        [LoggerMessage(EventId = 1021, Level = LogLevel.Debug, Message = "Attempting to pause hearing")]
        public static partial void LogPauseHearing(this ILogger logger);

        [LoggerMessage(EventId = 1022, Level = LogLevel.Debug, Message = "Attempting to end hearing")]
        public static partial void LogEndHearing(this ILogger logger);

        [LoggerMessage(EventId = 1023, Level = LogLevel.Debug, Message = "Attempting to transfer {Participant} into hearing room in {ConferenceId}")]
        public static partial void LogTransferParticipantIntoHearing(this ILogger logger, string participant, Guid conferenceId);

        [LoggerMessage(EventId = 1024, Level = LogLevel.Debug, Message = "Attempting to transfer {Participant} out of hearing room in {ConferenceId}")]
        public static partial void LogTransferParticipantOutOfHearing(this ILogger logger, string participant, Guid conferenceId);

        [LoggerMessage(EventId = 1025, Level = LogLevel.Warning, Message = "Unable to transfer Participant {Participant} in {ConferenceId}. Transfer type {TransferType} is unsupported")]
        public static partial void LogUnsupportedTransferType(this ILogger logger, string participant, Guid conferenceId, string transferType);

        [LoggerMessage(EventId = 1026, Level = LogLevel.Debug, Message = "Getting all active conferences")]
        public static partial void LogGettingActiveConferences(this ILogger logger);

        [LoggerMessage(EventId = 1027, Level = LogLevel.Warning, Message = "Unable to find participant requested by with id {RequestedBy}")]
        public static partial void LogParticipantRequestedByNotFound(this ILogger logger, Guid requestedBy);

        [LoggerMessage(EventId = 1028, Level = LogLevel.Warning, Message = "Unable to find participant requested for with id {RequestedFor}")]
        public static partial void LogParticipantRequestedForNotFound(this ILogger logger, Guid requestedFor);

        [LoggerMessage(EventId = 1029, Level = LogLevel.Warning, Message = "Please provide a room label")]
        public static partial void LogMissingRoomLabel(this ILogger logger);

        [LoggerMessage(EventId = 1030, Level = LogLevel.Information, Message = "Answered {Answer}")]
        public static partial void LogConsultationAnswer(this ILogger logger, string answer);

        [LoggerMessage(EventId = 1031, Level = LogLevel.Warning, Message = "Unable to find endpoint {EndpointId}")]
        public static partial void LogEndpointNotFound(this ILogger logger, Guid endpointId);

        [LoggerMessage(EventId = 1032, Level = LogLevel.Warning, Message = "Unable to find requestedBy participant {RequestedById}")]
        public static partial void LogRequestedByParticipantNotFound(this ILogger logger, Guid requestedById);

        [LoggerMessage(EventId = 1033, Level = LogLevel.Warning, Message = "Endpoint does not have a linked participant")]
        public static partial void LogEndpointWithoutLinkedParticipant(this ILogger logger);

        [LoggerMessage(EventId = 1034, Level = LogLevel.Warning, Message = "Participant is not linked to requested endpoint")]
        public static partial void LogParticipantNotLinkedToEndpoint(this ILogger logger);

        [LoggerMessage(EventId = 1035, Level = LogLevel.Warning, Message = "Unable to find room {RoomLabel}")]
        public static partial void LogRoomNotFound(this ILogger logger, string roomLabel);

        [LoggerMessage(EventId = 1036, Level = LogLevel.Warning, Message = "Unable to join endpoint {EndpointId} to {RoomLabel}")]
        public static partial void LogUnableToJoinEndpointToRoom(this ILogger logger, Guid endpointId, string roomLabel);

        [LoggerMessage(EventId = 1037, Level = LogLevel.Warning, Message = "Unable to find participant request by id {ParticipantId}")]
        public static partial void LogParticipantRequestByNotFound(this ILogger logger, Guid participantId);

        [LoggerMessage(EventId = 1038, Level = LogLevel.Debug, Message = "Remove heartbeats for conferences over 14 days old.")]
        public static partial void LogRemovingOldHeartbeats(this ILogger logger);

        [LoggerMessage(EventId = 1039, Level = LogLevel.Information, Message = "Successfully removed heartbeats for conferences.")]
        public static partial void LogSuccessfullyRemovedHeartbeats(this ILogger logger);

        [LoggerMessage(EventId = 1040, Level = LogLevel.Error, Message = "Unable to find conference for hearing: {HearingId}")]
        public static partial void LogConferenceNotFoundByHearingError(this ILogger logger, Guid hearingId, Exception exception);

        [LoggerMessage(EventId = 1041, Level = LogLevel.Error, Message = "Unable to find QuickLink participant {UserName}")]
        public static partial void LogQuickLinkNotFoundByUserNameError(this ILogger logger, string userName);

        [LoggerMessage(EventId = 1042, Level = LogLevel.Error, Message = "RemoveConference {ConferenceId}")]
        public static partial void LogRemoveConferenceError(this ILogger logger, Exception exception, Guid conferenceId);
    }
}