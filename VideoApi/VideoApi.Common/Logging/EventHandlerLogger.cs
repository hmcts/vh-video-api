namespace VideoApi.Common.Logging
{
    using System;
    using Microsoft.Extensions.Logging;

    public static partial class EventHandlerLogger
    {
        [LoggerMessage(EventId = 3000, Level = LogLevel.Trace, Message = "Handling CallbackEvent ({@Request}) skipped due to result of ShouldHandleEvent")]
        public static partial void LogHandlingCallbackEventSkipped(this ILogger logger, object request);

        [LoggerMessage(EventId = 3007, Level = LogLevel.Debug, Message = "Handling command {CommandName}")]
        public static partial void LogHandlingCommand(this ILogger logger, string commandName);

        [LoggerMessage(EventId = 3008, Level = LogLevel.Debug, Message = "Handled command {CommandName} in {ElapsedMilliseconds}ms")]
        public static partial void LogHandledCommand(this ILogger logger, string commandName, long elapsedMilliseconds);
        
        [LoggerMessage(EventId = 3009, Level = LogLevel.Information, Message = "Close callback - {ConferenceId} {Tags}")]
        public static partial void LogCloseCallback(this ILogger logger, Guid conferenceId, string tags);

        [LoggerMessage(EventId = 3010, Level = LogLevel.Information, Message = "CountdownFinished callback - {ConferenceId}")]
        public static partial void LogCountdownFinishedCallback(this ILogger logger, Guid conferenceId);

        [LoggerMessage(EventId = 3011, Level = LogLevel.Information, Message = "Returning Witness {ParticipantId} to the WaitingRoom - {ConferenceId}")]
        public static partial void LogReturningWitnessToWaitingRoom(this ILogger logger, Guid participantId, Guid conferenceId);
    
        [LoggerMessage(EventId = 3012, Level = LogLevel.Information, Message = "Disconnected callback - {ConferenceId}/{ParticipantId}")]
        public static partial void LogDisconnectedCallback(this ILogger logger, Guid conferenceId, Guid participantId);
        
        [LoggerMessage(EventId = 3013, Level = LogLevel.Information, Message = "EndpointDisconnected callback - {ConferenceId}/{EndpointId}")]
        public static partial void LogEndpointDisconnectedCallback(this ILogger logger, Guid conferenceId, Guid endpointId);

        [LoggerMessage(EventId = 3014, Level = LogLevel.Information, Message = "Endpoint joined callback - {ConferenceId}/{EndpointId}")]
        public static partial void LogEndpointJoinedCallback(this ILogger logger, Guid conferenceId, Guid endpointId);

        [LoggerMessage(EventId = 3015, Level = LogLevel.Information, Message = "Vodafone integration enabled, transferring endpoint {EndpointId} to hearing room if in session")]
        public static partial void LogVodafoneIntegrationEnabled(this ILogger logger, Guid endpointId);

        [LoggerMessage(EventId = 3016, Level = LogLevel.Information, Message = "Conference {ConferenceId} state is {ConferenceState}")]
        public static partial void LogConferenceState(this ILogger logger, Guid conferenceId, string conferenceState);

        [LoggerMessage(EventId = 3017, Level = LogLevel.Information, Message = "Conference {ConferenceId} already in session, transferring endpoint {EndpointId} to hearing room")]
        public static partial void LogTransferringEndpointToHearingRoom(this ILogger logger, Guid conferenceId, Guid endpointId);
        [LoggerMessage(EventId = 3018, Level = LogLevel.Information, Message = "EndpointTransferred callback - {ConferenceId}/{EndpointId}")]
        public static partial void LogEndpointTransferredCallback(this ILogger logger, Guid conferenceId, Guid endpointId);

        [LoggerMessage(EventId = 3019, Level = LogLevel.Information, Message = "Joined callback - {ConferenceId}/{ParticipantId}")]
        public static partial void LogJoinedCallback(this ILogger logger, Guid conferenceId, Guid participantId);

        [LoggerMessage(EventId = 3020, Level = LogLevel.Information, Message = "Conference {ConferenceId} already in session, transferring participant {ParticipantId} to hearing room")]
        public static partial void LogTransferringParticipantToHearingRoom(this ILogger logger, Guid conferenceId, Guid participantId);
    
        [LoggerMessage(EventId = 3021, Level = LogLevel.Information, Message = "Leave callback received - {ConferenceId}/{ParticipantId}")]
        public static partial void LogLeaveCallbackReceived(this ILogger logger, Guid conferenceId, Guid participantId);
    
        [LoggerMessage(EventId = 3022, Level = LogLevel.Information, Message = "ParticipantJoining callback received - {ConferenceId}/{ParticipantId}")]
        public static partial void LogParticipantJoiningCallbackReceived(this ILogger logger, Guid conferenceId, Guid participantId);
    
        [LoggerMessage(EventId = 3023, Level = LogLevel.Information, Message = "Pause callback received - {ConferenceId}")]
        public static partial void LogPauseCallbackReceived(this ILogger logger, Guid conferenceId);
    
        [LoggerMessage(EventId = 3024, Level = LogLevel.Information, Message = "Recording connection failed callback - {ConferenceId}")]
        public static partial void LogRecordingConnectionCallbackFailed(this ILogger logger, Guid conferenceId);
        
        [LoggerMessage(EventId = 3025, Level = LogLevel.Information, Message = "Room Participant Joined callback received - {ConferenceId}/{ParticipantId} - {ParticipantState} - {Room} {RoomLabel} - {SourceRoom}")]
        public static partial void LogRoomParticipantCallbackReceived(this ILogger logger, Guid conferenceId, Guid participantId, string participantState, string room, string roomLabel, long sourceRoom);
    
        [LoggerMessage(EventId = 3026, Level = LogLevel.Information, Message = "Room Participant Transferred ({Iteration}) callback received - {ConferenceId}/{ParticipantId} - {FromRoom} {FromRoomLabel} - {ToRoom} {ToRoomLabel} - {NewStatus}")]
        public static partial void LogRoomParticipantTransferredCallbackReceived(this ILogger logger, int iteration, Guid conferenceId, long participantId, string fromRoom, string fromRoomLabel, string toRoom, string toRoomLabel, string newStatus);
    
        [LoggerMessage(EventId = 3027, Level = LogLevel.Information, Message = "Start callback received - {ConferenceId}")]
        public static partial void LogStartCallbackReceived(this ILogger logger, Guid conferenceId);
    
        [LoggerMessage(EventId = 3028, Level = LogLevel.Information, Message = "TelephoneJoined callback - {ConferenceId}/{ParticipantId}")]
        public static partial void LogTelephoneJoinedCallback(this ILogger logger, Guid conferenceId, Guid participantId);

        [LoggerMessage(EventId = 3029, Level = LogLevel.Information, Message = "Conference {ConferenceId} already in session, transferring telephone participant {ParticipantId} to hearing room")]
        public static partial void LogTransferringTelephoneParticipantToHearingRoom(this ILogger logger, Guid conferenceId, Guid participantId);

        [LoggerMessage(EventId = 3030, Level = LogLevel.Information, Message = "Transfer callback received - {ConferenceId} - {ParticipantId}/{ParticipantRoomId} - {FromRoom} {FromRoomLabel} - {ToRoom} {ToRoomLabel}")]
        public static partial void LogTransferCallbackReceived(this ILogger logger, Guid conferenceId, Guid participantId, long? participantRoomId, string fromRoom, string fromRoomLabel, string toRoom, string toRoomLabel);

        [LoggerMessage(EventId = 3031, Level = LogLevel.Error, Message = "Unable to find room {RoomLabel} in conference {ConferenceId}")]
        public static partial void LogRoomNotFound(this ILogger logger, Exception exception, string roomLabel, Guid conferenceId);

        [LoggerMessage(EventId = 3032, Level = LogLevel.Information, Message = "No participants left in room {RoomLabel} - transferring all endpoints to waiting room")]
        public static partial void LogNoParticipantsLeftInRoom(this ILogger logger, string roomLabel);

        [LoggerMessage(EventId = 3033, Level = LogLevel.Information, Message = "No other participants linked to endpoint {EndpointId} - transferring to waiting room")]
        public static partial void LogNoOtherParticipantsLinkedToEndpoint(this ILogger logger, Guid endpointId);
        
        [LoggerMessage(EventId = 3034, Level = LogLevel.Debug, Message = "Handling callback")]
        public static partial void LogHandlingCallback(this ILogger logger);
        
        [LoggerMessage(EventId = 3035, Level = LogLevel.Debug, Message = "Handled callback in {ElapsedMilliseconds}ms")]
        public static partial void LogHandlingTimeCallback(this ILogger logger, long elapsedMilliseconds);
        
        [LoggerMessage(EventId = 3036, Level = LogLevel.Information, Message = "Suspend callback received - {ConferenceId}")]
        public static partial void LogSuspendCallbackReceived(this ILogger logger, Guid conferenceId);

        [LoggerMessage(EventId = 3037, Level = LogLevel.Information, Message = "TelephoneDisconnected callback - {ConferenceId}/{TelephoneParticipantId}")]
        public static partial void LogTelephoneDisconnectedCallback(this ILogger logger, Guid conferenceId, Guid telephoneParticipantId);

        [LoggerMessage(EventId = 3038, Level = LogLevel.Information, Message = "TelephoneTransferred callback - {ConferenceId}/{TelephoneParticipantId} from {FromRoomLabel} to {ToRoomLabel}")]
        public static partial void LogTelephoneTransferredCallback(this ILogger logger, Guid conferenceId, Guid telephoneParticipantId, string fromRoomLabel, string toRoomLabel);

        [LoggerMessage(EventId = 3039, Level = LogLevel.Information, Message = "Request to {RequestUri}: {RequestBody}")]
        public static partial void LogRequestDetails(this ILogger logger, string requestUri, string requestBody);
        
        [LoggerMessage(EventId = 3040, Level = LogLevel.Error, Message = "Unexpected event order for participant")]
        public static partial void LogUnexpectedEventOrderForParticipant(this ILogger logger, Exception exception);

        [LoggerMessage(EventId = 3041, Level = LogLevel.Error, Message = "Unexpected event order for endpoint")]
        public static partial void LogUnexpectedEventOrderForEndpoint(this ILogger logger, Exception exception);

        [LoggerMessage(EventId = 3042, Level = LogLevel.Error, Message = "Unexpected event order for telephone participant")]
        public static partial void LogUnexpectedEventOrderForTelephoneParticipant(this ILogger logger, Exception exception);
        
        [LoggerMessage(EventId = 3043, Level = LogLevel.Error, Message = "Room already booked for conference {ConferenceId}")]
        public static partial void LogRoomAlreadyBooked(this ILogger logger, Exception exception, Guid conferenceId);
    }
}