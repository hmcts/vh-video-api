using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;

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

        public UpdateParticipantDetailsCommand(Guid conferenceId, Guid participantId, string fullname, string firstname,
            string lastname, string displayName, string representee, string contactEmail, string contactTelephone)
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
            var conference = await _context.Conferences.Include("Participants").SingleOrDefaultAsync(x => x.Id == command.ConferenceId);
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
            
            await _context.SaveChangesAsync();
        }
    }
}
