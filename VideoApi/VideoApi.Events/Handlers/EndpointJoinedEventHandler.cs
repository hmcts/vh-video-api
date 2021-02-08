using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;

namespace VideoApi.Events.Handlers
{
    public class EndpointJoinedEventHandler : EventHandlerBase<EndpointJoinedEventHandler>
    {
        public EndpointJoinedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<EndpointJoinedEventHandler> logger) : base(
            queryHandler, commandHandler, logger)
        {
        }

        public override EventType EventType => EventType.EndpointJoined;

        protected override Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            const EndpointState endpointState = EndpointState.Connected;
            const RoomType room = RoomType.WaitingRoom;
            var command = new UpdateEndpointStatusAndRoomCommand(SourceConference.Id, SourceEndpoint.Id, endpointState, room);
            return CommandHandler.Handle(command);
        }
    }
}
