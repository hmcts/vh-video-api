using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Commands
{
    public class AddTaskCommand : ICommand
    {
        public Guid ConferenceId { get; }
        public Guid OriginId { get; }
        public string Body { get; }
        public TaskType TaskType { get; }

        public AddTaskCommand(Guid conferenceId, Guid originId, string body, TaskType taskType)
        {
            ConferenceId = conferenceId;
            OriginId = originId;
            Body = body;
            TaskType = taskType;
        }
    }

    public class AddTaskCommandHandler : ICommandHandler<AddTaskCommand>
    {
        private readonly VideoApiDbContext _context;

        public AddTaskCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddTaskCommand command)
        {
            var conference = await _context.Conferences.SingleOrDefaultAsync(x => x.Id == command.ConferenceId);
            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            var task = new Domain.Task(command.ConferenceId, command.OriginId, command.Body, command.TaskType);
            await _context.Tasks.AddAsync(task);

            await _context.SaveChangesAsync();
        }
    }
}
