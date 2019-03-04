using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;

namespace VideoApi.DAL.Commands
{
    public class AddParticipantsToConferenceCommand : ICommand
    {
        public Guid ConferenceId { get; set; }
        public List<Participant> Participants { get; set; }

        public AddParticipantsToConferenceCommand(Guid conferenceId, List<Participant> participants)
        {
            ConferenceId = conferenceId;
            Participants = participants;
        }
    }

    public class AddParticipantsToConferenceCommandHandler : ICommandHandler<AddParticipantsToConferenceCommand>
    {
        private readonly VideoApiDbContext _context;

        public AddParticipantsToConferenceCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddParticipantsToConferenceCommand command)
        {
            var conference = await _context.Conferences.Include("Participants")
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            foreach (var participant in command.Participants)
            {
                conference.AddParticipant(participant);
            }
            await _context.SaveChangesAsync();
        }
    }
}