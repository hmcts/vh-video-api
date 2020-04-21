using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;

namespace VideoApi.DAL.Commands
{
    public class UpdateTaskCommand : ICommand
    {
        public Guid ConferenceId { get; }
        public long TaskId { get; }
        public string UpdatedBy { get; set; }
        public UpdateTaskCommand(Guid conferenceId, long taskId, string updatedBy)
        {
            ConferenceId = conferenceId;
            TaskId = taskId;
            UpdatedBy = updatedBy;
        }
    }

    public class UpdateTaskCommandHandler : ICommandHandler<UpdateTaskCommand>
    {
        private readonly VideoApiDbContext _context;

        public UpdateTaskCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateTaskCommand command)
        {
            var task = _context.Tasks
                .Where(x => x.Id == command.TaskId && x.ConferenceId == command.ConferenceId)
                .SingleOrDefault();

            if (task == null)
            {
                throw new TaskNotFoundException(command.ConferenceId, command.TaskId);
            }

            task.CompleteTask(command.UpdatedBy);
            await _context.SaveChangesAsync();
        }
    }
}
