using System;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class CloseConferenceCommand : ICommand
    {
        public Guid ConferenceId { get; }

        public CloseConferenceCommand(Guid conferenceId)
        {
            ConferenceId = conferenceId;
        }
    }

    public class CloseConferenceCommandHandler : ICommandHandler<CloseConferenceCommand>
    {
        private readonly VideoApiDbContext _context;

        public CloseConferenceCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(CloseConferenceCommand command)
        {
            var conference = await _context.Conferences
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            conference.CloseConference();
            
            await _context.SaveChangesAsync();
        }
    }
}
