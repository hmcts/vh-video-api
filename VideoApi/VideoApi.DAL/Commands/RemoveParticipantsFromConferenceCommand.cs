using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using System.Linq;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class RemoveParticipantsFromConferenceCommand : ICommand
    {
        public Guid ConferenceId { get; set; }
        public List<ParticipantBase> Participants { get; set; }

        public RemoveParticipantsFromConferenceCommand(Guid conferenceId, List<ParticipantBase> participants)
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
            var conference = await _context.Conferences.Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants)
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            foreach (var participant in command.Participants)
            {
                // remove all linked participants where the current participant is the secondary, i.e., LinkedId
                //
                // get the Ids of the participants the primary participant is linked to
                var linkedParticipantIds = participant.LinkedParticipants.Select(x => x.LinkedId);
                // get the participant for each of the Ids
                var linkedParticipants = await _context.Participants.Include(x => x.LinkedParticipants)
                    .Where(x => linkedParticipantIds.Contains(x.Id)).ToListAsync();
                // for each one of these participants, remove the linked participant that refers to the primary participant
                foreach (var linkedParticipant in linkedParticipants)
                {
                    // get all linked participants between the secondary participant and primary participant
                    var linkedParticipantsFromSecondary = linkedParticipant.LinkedParticipants.Where(x =>
                        x.ParticipantId == linkedParticipant.Id && x.LinkedId == participant.Id).ToList();

                    // remove each one
                    foreach (var linkedParticipantFromSecondary in linkedParticipantsFromSecondary)
                    {
                        linkedParticipant.RemoveLink(linkedParticipantFromSecondary);
                    }
                }

                // remove the linked participants where the current participant is primary
                participant.RemoveAllLinks();
                
                conference.RemoveParticipant(participant);
            }

            await _context.SaveChangesAsync();
        }
    }
}
