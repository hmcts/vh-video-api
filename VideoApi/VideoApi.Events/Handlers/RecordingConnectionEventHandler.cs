using Microsoft.Extensions.Logging;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using Task = System.Threading.Tasks.Task;
using VideoApi.Common.Logging;

namespace VideoApi.Events.Handlers
{
    public class RecordingConnectionEventHandler : EventHandlerBase<RecordingConnectionEventHandler>
    {
        public RecordingConnectionEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<RecordingConnectionEventHandler> logger) : base(queryHandler, commandHandler, logger)
        {
        }

        public override EventType EventType => EventType.RecordingConnectionFailed;
        
        protected override Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            Logger.LogInformationRecordingConnectionFailedCallback(SourceConference.Id);
            return Task.CompletedTask;
        }
    }
}
