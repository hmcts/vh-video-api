using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;

namespace VideoApi.DAL.Commands
{
    public class CompleteAlertCommand : ICommand
    {
        public Guid ConferenceId { get; }
        public long AlertId { get; }
        public string UpdatedBy { get; }

        public CompleteAlertCommand(Guid conferenceId, long alertId, string updatedBy)
        {
            AlertId = alertId;
            UpdatedBy = updatedBy;
            ConferenceId = conferenceId;
        }
    }
    
    public class CompleteTaskCommandHandler : ICommandHandler<CompleteAlertCommand>
    {
        private readonly VideoApiDbContext _context;

        public CompleteTaskCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(CompleteAlertCommand command)
        {
            var conference = await _context.Conferences.Include(x => x.Tasks)
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);
            
            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            var alert = conference.GetTasks().FirstOrDefault(x => x.Id == command.AlertId);

            if (alert == null)
            {
                throw new AlertNotFoundException(command.ConferenceId, command.AlertId);
            }
            
            alert.CompleteTask(command.UpdatedBy.Trim());
            await _context.SaveChangesAsync();
        }
    }
}