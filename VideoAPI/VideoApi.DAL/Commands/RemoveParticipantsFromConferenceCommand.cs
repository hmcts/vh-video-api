using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;

namespace VideoApi.DAL.Commands
{
    public class RemoveParticipantsFromConferenceCommand : ICommand
    {
        public Guid ConferenceId { get; set; }
        public List<Participant> Participants { get; set; }

        public RemoveParticipantsFromConferenceCommand(Guid conferenceId, List<Participant> participants)
        {
            ConferenceId = conferenceId;
            Participants = participants;
        }
    }

    public class RemoveParticipantsFromConferenceCommandHandler : ICommandHandler<RemoveParticipantsFromConferenceCommand>
    {
        private readonly VideoApiDbContext _context;

        public RemoveParticipantsFromConferenceCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(RemoveParticipantsFromConferenceCommand command)
        {
            var conference = await _context.Conferences.Include("Participants")
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            foreach (var participant in command.Participants)
            {
                conference.RemoveParticipant(participant);
            }
            await _context.SaveChangesAsync();
        }
    }
}