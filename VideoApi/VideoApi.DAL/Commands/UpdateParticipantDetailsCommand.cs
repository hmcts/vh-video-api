using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VideoApi.Domain.Enums;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.DTOs;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class UpdateParticipantDetailsCommand : ICommand
    {
        public Guid ConferenceId { get; }
        public Guid ParticipantId { get; }
        public string DisplayName { get; }
        public string ContactEmail { get; }
        public string Username { get; set; }
        public IList<LinkedParticipantDto> LinkedParticipants { get; set; }

        public UserRole UserRole { get; set; }
        public string HearingRole { get; set; }

        public UpdateParticipantDetailsCommand(Guid conferenceId,
            Guid participantId,
            string displayName, string contactEmail, 
            IList<LinkedParticipantDto> linkedParticipants, UserRole userRole, string hearingRole)
        {
            ConferenceId = conferenceId;
            ParticipantId = participantId;
            DisplayName = displayName;
            ContactEmail = contactEmail;
            LinkedParticipants = linkedParticipants;
            UserRole = userRole;
            HearingRole = hearingRole;
        }
    }

    public class UpdateParticipantDetailsCommandHandler : ICommandHandler<UpdateParticipantDetailsCommand>
    {
        private readonly VideoApiDbContext _context;

        public UpdateParticipantDetailsCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateParticipantDetailsCommand command)
        {
            var conference = await _context.Conferences.Include(x => x.Participants).ThenInclude(x => x.LinkedParticipants).SingleOrDefaultAsync(x => x.Id == command.ConferenceId);
            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            var participant = conference.GetParticipants().SingleOrDefault(x => x.Id == command.ParticipantId);
            if (participant == null)
            {
                throw new ParticipantNotFoundException(command.ParticipantId);
            }
            
            participant.DisplayName = command.DisplayName;
            participant.Username = command.Username ?? participant.Username;

            if (participant is Participant participantCasted)
            {
                participantCasted.ContactEmail = command.ContactEmail ?? participantCasted.ContactEmail;
                
                if (command.UserRole != UserRole.None)
                    participantCasted.UserRole = command.UserRole;

                if (command.HearingRole != null)
                    participantCasted.HearingRole = command.HearingRole;
            }

            await UpdateLinkedParticipants(participant, conference, command);
   
            await _context.SaveChangesAsync();
        }

        private async Task UpdateLinkedParticipants(ParticipantBase participant,
            Conference conference,
            UpdateParticipantDetailsCommand command)
        {
            // remove all linked participants where the current participant is the secondary, i.e., LinkedId
            //
            // get the Ids of the participants the primary participant is linked to
            var linkedParticipantIds = participant.LinkedParticipants.Select(x => x.LinkedId);
            // get the participant for each of the Ids
            var linkedParticipants = await _context.Participants.Where(x => linkedParticipantIds.Contains(x.Id)).ToListAsync();
            // for each one of these participants, remove the linked participant that refers to the primary participant
            foreach (var linkedParticipant in linkedParticipants)
            {
                // get all linked participants between the secondary participant and primary participant
                var linkedParticipantsFromSecondary = 
                    linkedParticipant.LinkedParticipants
                        .Where(x => x.ParticipantId == linkedParticipant.Id && x.LinkedId == participant.Id)
                        .ToList();
                
                // remove each one
                foreach (var linkedParticipantFromSecondary in linkedParticipantsFromSecondary)
                {
                    linkedParticipant.RemoveLink(linkedParticipantFromSecondary);   
                }
            }
            
            // remove the linked participants where the current participant is primary
            participant.RemoveAllLinks();

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
        }
    }
}
