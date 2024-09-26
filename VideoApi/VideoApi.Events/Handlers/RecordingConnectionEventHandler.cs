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
    public class RecordingConnectionEventHandler : EventHandlerBase<CountdownFinishedEventHandler>
    {
        private readonly IConsultationService _consultationService;
        public RecordingConnectionEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<CountdownFinishedEventHandler> logger, IConsultationService consultationService) : base(queryHandler, commandHandler, logger)
        {
            _consultationService = consultationService;
        }

        public override EventType EventType => EventType.RecordingConnectionFailed;
        
        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            
        }
    }
}
