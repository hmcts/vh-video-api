using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.DTOs;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class AddParticipantsToConferenceCommand : ICommand
    {
        public Guid ConferenceId { get; set; }
        public List<ParticipantBase> Participants { get; set; }
        public List<LinkedParticipantDto> LinkedParticipants { get; set; }

        public AddParticipantsToConferenceCommand(Guid conferenceId, List<ParticipantBase> participants, List<LinkedParticipantDto> linkedParticipants)
        {
            ConferenceId = conferenceId;
            Participants = participants;
            LinkedParticipants = linkedParticipants;
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
            var conference = await _context.Conferences.Include(x => x.Participants)
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            foreach (var participant in command.Participants)
            {
                conference.AddParticipant(participant);
                _context.Entry(participant).State = EntityState.Added;
            }

            foreach (var linkedParticipant in command.LinkedParticipants)
            {
                try
                {
                    var primaryParticipant =
                        conference.Participants.Single(x => x.ParticipantRefId == linkedParticipant.ParticipantRefId);
                
                    var secondaryParticipant =
                        conference.Participants.Single(x => x.ParticipantRefId == linkedParticipant.LinkedRefId);
                    
                    primaryParticipant.AddLink(secondaryParticipant.Id, linkedParticipant.Type);
                }
                catch (Exception)
                {
                    throw new ParticipantLinkException(linkedParticipant.ParticipantRefId, linkedParticipant.LinkedRefId);
                }
            }
            
            await _context.SaveChangesAsync();
        }
    }
}
