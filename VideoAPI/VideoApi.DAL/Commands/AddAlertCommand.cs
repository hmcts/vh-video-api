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
        public AlertType AlertType { get; }
        
        public AddAlertCommand(Guid conferenceId, string body, AlertType alertType)
        {
            ConferenceId = conferenceId;
            Body = body;
            AlertType = alertType;
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
            var conference = await _context.Conferences.Include(x => x.Alerts)
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }
            
            conference.AddAlert(command.AlertType, command.Body);

            await _context.SaveChangesAsync();
        }
    }
}