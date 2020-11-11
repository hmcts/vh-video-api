using System.Threading.Tasks;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;

namespace VideoApi.Events.Handlers
{
    public class SelectingMediaEventHandler : EventHandlerBase
    {
        public SelectingMediaEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler) : base(queryHandler, commandHandler)
        {
        }

        public override EventType EventType => EventType.SelectingMedia;
        protected override Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            // simply a logging event
            return Task.CompletedTask;
        }
    }
}
