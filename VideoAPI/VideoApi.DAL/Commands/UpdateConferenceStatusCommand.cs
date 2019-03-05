using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Commands
{
    public class UpdateConferenceStatusCommand : ICommand
    {
        public Guid ConferenceId { get; set; }
        public ConferenceState ConferenceState { get; set; }

        public UpdateConferenceStatusCommand(Guid conferenceId, ConferenceState conferenceState)
        {
            ConferenceId = conferenceId;
            ConferenceState = conferenceState;
        }
    }

    public class UpdateConferenceStatusCommandHandler : ICommandHandler<UpdateConferenceStatusCommand>
    {
        private readonly VideoApiDbContext _context;

        public UpdateConferenceStatusCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateConferenceStatusCommand command)
        {
            var conference = await _context.Conferences.Include("ConferenceStatuses")
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            conference.UpdateConferenceStatus(command.ConferenceState);
            await _context.SaveChangesAsync();
        }
    }
}