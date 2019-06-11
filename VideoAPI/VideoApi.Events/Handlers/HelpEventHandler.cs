using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Hub;
using VideoApi.Events.Models;
using VideoApi.Events.ServiceBus;

namespace VideoApi.Events.Handlers
{
    public class HelpEventHandler : EventHandlerBase
    {
        public HelpEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IServiceBusQueueClient serviceBusQueueClient, IHubContext<EventHub, IEventHubClient> hubContext) : base(
            queryHandler, commandHandler, serviceBusQueueClient, hubContext)
        {
        }

        public override EventType EventType => EventType.Help;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            await HubContext.Clients.Group(EventHub.VhOfficersGroupName)
                .HelpMessage(SourceConference.Id, SourceParticipant.DisplayName);
        }
    }
}