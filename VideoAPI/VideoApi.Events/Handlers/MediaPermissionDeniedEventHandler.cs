using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Amqp.Serialization;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
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
            var query = new GetTasksForConferenceQuery(SourceConference.Id);
            var tasks = await QueryHandler.Handle<GetTasksForConferenceQuery, List<Domain.Task>>(query);
            var task = tasks.SingleOrDefault(x => x.Type == TaskType.Participant && x.OriginId == SourceParticipant.Id);

            if (task == null)
            {
                var participant = SourceConference.Participants.Single(x => x.Id == SourceParticipant.Id);
                var command = new AddTaskCommand(SourceConference.Id, participant.Name, TaskType.Participant);
                await CommandHandler.Handle(command);
            }
        }
    }
}