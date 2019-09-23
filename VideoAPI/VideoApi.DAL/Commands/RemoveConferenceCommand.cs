using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;

namespace VideoApi.DAL.Commands
{
    public class RemoveConferenceCommand : ICommand
    {
        public RemoveConferenceCommand(Guid conferenceId)
        {
            ConferenceId = conferenceId;
        }

        public Guid ConferenceId { get; set; }
    }

    public class RemoveConferenceCommandHandler : ICommandHandler<RemoveConferenceCommand>
    {
        private readonly VideoApiDbContext _context;

        public RemoveConferenceCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(RemoveConferenceCommand command)
        {
            var conference = await _context.Conferences
                .Include("Participants.ParticipantStatuses")
                .Include("ConferenceStatuses")
                .Include(x => x.Participants).ThenInclude(x => x.TestCallResult)
                .Include(x => x.Tasks)
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);
            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }
            var events = await _context.Events.Where(x => x.ConferenceId == conference.Id).ToListAsync();

            _context.Remove(conference);
            _context.RemoveRange(events);
            
            await _context.SaveChangesAsync();
        }
    }
}