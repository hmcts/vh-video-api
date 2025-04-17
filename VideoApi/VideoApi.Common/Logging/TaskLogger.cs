namespace VideoApi.Common.Logging
{
    using System;
    using Microsoft.Extensions.Logging;

    public static partial class TaskLogger
    {
        [LoggerMessage(EventId = 8000, Level = LogLevel.Debug, Message = "GetTasksForConference")]
        public static partial void LogGetTasksForConference(this ILogger logger);

        [LoggerMessage(EventId = 8001, Level = LogLevel.Error, Message = "Unable to find tasks")]
        public static partial void LogUnableToFindTasks(this ILogger logger, Exception exception);

        [LoggerMessage(EventId = 8002, Level = LogLevel.Debug, Message = "UpdateTaskStatus")]
        public static partial void LogUpdateTaskStatus(this ILogger logger);

        [LoggerMessage(EventId = 8003, Level = LogLevel.Error, Message = "Unable to find task")]
        public static partial void LogUnableToFindTask(this ILogger logger, Exception exception);

        [LoggerMessage(EventId = 8004, Level = LogLevel.Debug, Message = "Adding a task {Body} for participant {ParticipantId} in conference {ConferenceId}")]
        public static partial void LogAddingTask(this ILogger logger, string body, Guid participantId, Guid conferenceId);

        [LoggerMessage(EventId = 8005, Level = LogLevel.Error, Message = "Unable to add a task {Body} for participant {ParticipantId} in conference {ConferenceId}")]
        public static partial void LogUnableToAddTask(this ILogger logger, string body, Guid participantId, Guid conferenceId, Exception exception);

        [LoggerMessage(EventId = 8006, Level = LogLevel.Error, Message = "Unable to find task {TaskId}")]
        public static partial void LogUnableToFindTaskId(this ILogger logger, long taskId);
    }
}