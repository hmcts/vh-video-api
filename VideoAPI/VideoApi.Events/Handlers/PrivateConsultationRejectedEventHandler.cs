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
    public class PrivateConsultationRejectedEventHandler : EventHandlerBase
    {
        public PrivateConsultationRejectedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler) :
            base(queryHandler, commandHandler)
        {
        }

        public override EventType EventType => EventType.PrivateConsultationRejected;

        protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var query = new GetTasksForConferenceQuery(SourceConference.Id);
            var taskType = SourceParticipant.IsJudge() ? TaskType.Judge : TaskType.Participant;
            var tasks = await QueryHandler.Handle<GetTasksForConferenceQuery, List<Domain.Task>>(query);
            var task = tasks.SingleOrDefault(x =>
                x.Type == taskType && x.OriginId == SourceParticipant.Id && x.Status == TaskStatus.ToDo &&
                x.Body.Contains("rejected private consultation"));

            if (task == null)
            {
                var command = new AddTaskCommand(SourceConference.Id, SourceParticipant.Id, callbackEvent.Reason,
                    taskType);
                await CommandHandler.Handle(command);
            }
        }
    }
}
