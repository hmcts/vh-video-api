using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;

namespace VideoApi.Events.Handlers
{
    public class EndpointJoinedEventHandler : EventHandlerBase
    {
        public EndpointJoinedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler) : base(queryHandler, commandHandler)
        {
        }

        public override EventType EventType => EventType.EndpointJoined;
        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            const EndpointState endpointState = EndpointState.Connected;
            var command = new UpdateEndpointStatusCommand(SourceConference.Id, SourceEndpoint.Id, endpointState);
            await CommandHandler.Handle(command);
        }
    }
}
