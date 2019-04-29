using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Commands
{
    public class AddAlertCommand : ICommand
    {
        public Guid ConferenceId { get; }
        public string Body { get; }
        public TaskType TaskType { get; }

        public AddAlertCommand(Guid conferenceId, string body, TaskType taskType)
        {
            ConferenceId = conferenceId;
            Body = body;
            TaskType = taskType;
        }
    }

    public class AddAlertCommandHandler : ICommandHandler<AddAlertCommand>
    {
        private readonly VideoApiDbContext _context;

        public AddAlertCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddAlertCommand command)
        {
            var conference = await _context.Conferences.Include(x => x.Tasks)
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }
            
            conference.AddTask(command.TaskType, command.Body);

            await _context.SaveChangesAsync();
        }
    }
}