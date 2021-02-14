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
    public class UpdateParticipantDetailsCommand : ICommand
    {
        public Guid ConferenceId { get; }
        public Guid ParticipantId { get; }
        public string DisplayName { get; }
        public string Representee { get; }
        public string Fullname { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string ContactEmail { get; }
        public string ContactTelephone { get; }
        public string Username { get; set; }
        public IList<LinkedParticipantDto> LinkedParticipants { get; set; }

        public UpdateParticipantDetailsCommand(Guid conferenceId, Guid participantId, string fullname, string firstname,
            string lastname, string displayName, string representee, string contactEmail, string contactTelephone, 
            IList<LinkedParticipantDto> linkedParticipants)
        {
            ConferenceId = conferenceId;
            ParticipantId = participantId;
            Fullname = fullname;
            DisplayName = displayName;
            Representee = representee;
            FirstName = firstname;
            LastName = lastname;
            ContactEmail = contactEmail;
            ContactTelephone = contactTelephone;
            LinkedParticipants = linkedParticipants;
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

            participant.Name = command.Fullname;
            participant.FirstName = command.FirstName;
            participant.LastName = command.LastName;
            participant.DisplayName = command.DisplayName;
            participant.Representee = command.Representee;
            
            participant.ContactEmail = command.ContactEmail ?? participant.ContactEmail;
            participant.ContactTelephone = command.ContactTelephone ?? participant.ContactTelephone;
            participant.Username = command.Username ?? participant.Username;

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
