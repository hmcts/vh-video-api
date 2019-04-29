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
        public long AlertId { get; }
        public string UpdatedBy { get; set; }
        public UpdateTaskCommand(Guid conferenceId, long alertId, string updatedBy)
        {
            ConferenceId = conferenceId;
            AlertId = alertId;
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
            var alert = await _context.Conferences.Include(x => x.Tasks)
                .Where(x => x.Id == command.ConferenceId)
                .SelectMany(x => x.Tasks)
                .SingleOrDefaultAsync(x => x.Id == command.AlertId);

            if (alert == null)
            {
                throw new AlertNotFoundException(command.ConferenceId, command.AlertId);
            }

            alert.CompleteTask(command.UpdatedBy);
            await _context.SaveChangesAsync();
        }
    }
}