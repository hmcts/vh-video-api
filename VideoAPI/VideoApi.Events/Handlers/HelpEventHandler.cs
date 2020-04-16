using System.Threading.Tasks;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;

namespace VideoApi.Events.Handlers
{
    public class HelpEventHandler : EventHandlerBase
    {
        public HelpEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler) : base(
            queryHandler, commandHandler)
        {
        }
        public override EventType EventType => EventType.Help;

        public override async Task HandleAsync(CallbackEvent callbackEvent)
        {
            // We don't do anything with this
        }

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            // We don't do anything with this
        }

    }
}
