using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Hub;
using VideoApi.Events.Models;
using VideoApi.Events.ServiceBus;

namespace VideoApi.Events.Handlers
{
    public class MediaPermissionDeniedEventHandler : EventHandlerBase
    {
        public MediaPermissionDeniedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IServiceBusQueueClient serviceBusQueueClient, IHubContext<EventHub, IEventHubClient> hubContext) : base(
            queryHandler, commandHandler, serviceBusQueueClient, hubContext)
        {
        }

        public override EventType EventType => EventType.MediaPermissionDenied;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var participant = SourceConference.Participants.Single(x => x.Id == SourceParticipant.Id);
            var command = new AddAlertCommand(SourceConference.Id, participant.Name, TaskType.Participant);
            await CommandHandler.Handle(command);
        }
    }
}