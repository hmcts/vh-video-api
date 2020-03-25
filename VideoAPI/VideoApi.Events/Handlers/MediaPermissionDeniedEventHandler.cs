using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using TaskStatus = VideoApi.Domain.Enums.TaskStatus;

namespace VideoApi.Events.Handlers
{
    public class MediaPermissionDeniedEventHandler : EventHandlerBase
    {
        public MediaPermissionDeniedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler) : base(
            queryHandler, commandHandler)
        {
        }

        public override EventType EventType => EventType.MediaPermissionDenied;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var query = new GetTasksForConferenceQuery(SourceConference.Id);
            var tasks = await QueryHandler.Handle<GetTasksForConferenceQuery, List<Domain.Task>>(query);
            var task = tasks.SingleOrDefault(x =>
                x.Type == TaskType.Participant && x.OriginId == SourceParticipant.Id && x.Status != TaskStatus.ToDo);

            if (task == null)
            {
                var command = new AddTaskCommand(SourceConference.Id, SourceParticipant.Id, "Media blocked",
                    TaskType.Participant);
                await CommandHandler.Handle(command);
            }
        }
    }
}
