using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;

namespace VideoApi.DAL.Commands
{
    public class UpdateAlertCommand : ICommand
    {
        public Guid ConferenceId { get; }
        public long AlertId { get; }
        public string UpdatedBy { get; set; }
        public UpdateAlertCommand(Guid conferenceId, long alertId, string updatedBy)
        {
            ConferenceId = conferenceId;
            AlertId = alertId;
            UpdatedBy = updatedBy;
        }
    }

    public class UpdateAlertCommandHandler : ICommandHandler<UpdateAlertCommand>
    {
        private readonly VideoApiDbContext _context;

        public UpdateAlertCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateAlertCommand command)
        {
            var alert = await _context.Conferences.Include(x => x.Alerts)
                .Where(x => x.Id == command.ConferenceId)
                .SelectMany(x => x.Alerts)
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