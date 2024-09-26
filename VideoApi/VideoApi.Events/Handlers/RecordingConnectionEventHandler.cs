using System.Linq;
using Microsoft.Extensions.Logging;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using VideoApi.Services.Contracts;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Events.Handlers
{
    public class RecordingConnectionEventHandler : EventHandlerBase<RecordingConnectionEventHandler>
    {
        public RecordingConnectionEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<RecordingConnectionEventHandler> logger) : base(queryHandler, commandHandler, logger)
        {
        }

        public override EventType EventType => EventType.RecordingConnectionFailed;
        
        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            _logger.LogInformation("Start callback received - {ConferenceId}",
                SourceConference.Id);
        }
    }
}
