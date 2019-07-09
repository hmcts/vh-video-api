using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Hub;
using VideoApi.Events.Models;
using VideoApi.Events.ServiceBus;
using TaskStatus = VideoApi.Domain.Enums.TaskStatus;

namespace VideoApi.Events.Handlers
{
    public class SelfTestFailedEventHandler : EventHandlerBase
    {
        public SelfTestFailedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler,
            IServiceBusQueueClient serviceBusQueueClient, IHubContext<EventHub, IEventHubClient> hubContext) : base(
            queryHandler, commandHandler, serviceBusQueueClient, hubContext)
        {
        }

        public override EventType EventType => EventType.SelfTestFailed;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var query = new GetTasksForConferenceQuery(SourceConference.Id);
            var tasks = await QueryHandler.Handle<GetTasksForConferenceQuery, List<Domain.Task>>(query);
            var task = tasks.SingleOrDefault(x => x.Type == TaskType.Participant 
                && x.OriginId == SourceParticipant.Id && x.Status != TaskStatus.ToDo && x.Body == callbackEvent.Reason);
            if (task == null)
            {
                var command = new AddTaskCommand(SourceConference.Id, SourceParticipant.Id,
                    callbackEvent.Reason, TaskType.Participant);
                await CommandHandler.Handle(command);
            }
        }
    }
}
