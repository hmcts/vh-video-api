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
    public class UpdateConferenceParticipantsCommand : ICommand
    {
        public Guid ConferenceId { get; set; }
        public IList<ParticipantBase> ExistingParticipants { get; set; }
        public IList<ParticipantBase> NewParticipants { get; set; }
        public IList<Guid> RemovedParticipantRefIds { get; set; }
        public IList<LinkedParticipantDto> LinkedParticipants { get; set; }

        public UpdateConferenceParticipantsCommand(
            Guid conferenceId,
            IList<ParticipantBase> existingParticipants,
            IList<ParticipantBase> newParticipants,
            IList<Guid> removedParticipants,
            IList<LinkedParticipantDto> linkedParticipants)
        {
            ConferenceId = conferenceId;
            ExistingParticipants = existingParticipants;
            NewParticipants = newParticipants;
            RemovedParticipantRefIds = removedParticipants;
            LinkedParticipants = linkedParticipants;
        }
    }

    public class UpdateConferencParticipantsCommandHandler : ICommandHandler<UpdateConferenceParticipantsCommand>
    {
        private readonly VideoApiDbContext _context;

        public UpdateConferencParticipantsCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateConferenceParticipantsCommand command)
        {
            var conference = await _context.Conferences.Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants).SingleOrDefaultAsync(x => x.Id == command.ConferenceId);
            
            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            foreach (var participant in command.NewParticipants)
            {
                conference.AddParticipant(participant);
                _context.Entry(participant).State = EntityState.Added;
            }

            foreach (var existingParticipant in command.ExistingParticipants)
            {
                var participant = conference.GetParticipants().SingleOrDefault(x => x.ParticipantRefId == existingParticipant.ParticipantRefId);
                
                if (participant == null)
                {
                    throw new ParticipantNotFoundException(existingParticipant.ParticipantRefId);
                }

                participant.DisplayName = existingParticipant.DisplayName;
                participant.Name = existingParticipant.Name;
                participant.Username = existingParticipant.Username ?? participant.Username;

                if (participant is Participant participantCasted)
                {
                    participantCasted.ContactEmail = ((Participant)existingParticipant).ContactEmail ?? participantCasted.ContactEmail;
                    participantCasted.ContactTelephone = ((Participant)existingParticipant).ContactTelephone ?? participantCasted.ContactTelephone;
                    participantCasted.FirstName = ((Participant)existingParticipant).FirstName;
                    participantCasted.LastName = ((Participant)existingParticipant).LastName;
                    participantCasted.Representee = ((Participant)existingParticipant).Representee;
                }

                participant.RemoveAllLinks();
            }

            foreach (var removedParticipantRefId in command.RemovedParticipantRefIds)
            {
                var participant = conference.GetParticipants().SingleOrDefault(x => x.ParticipantRefId == removedParticipantRefId);
                
                if (participant == null)
                {
                    throw new ParticipantNotFoundException(removedParticipantRefId);
                }

                participant.RemoveAllLinks();
                conference.RemoveParticipant(participant);
            }

            foreach (var linkedParticipant in command.LinkedParticipants)
            {
                try
                {
                    conference.AddLinkedParticipants(linkedParticipant.ParticipantRefId, linkedParticipant.LinkedRefId, linkedParticipant.Type);
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
