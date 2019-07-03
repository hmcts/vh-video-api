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

        public UpdateParticipantDetailsCommand(Guid conferenceId, Guid participantId, string fullname,
            string displayName, string representee)
        {
            ConferenceId = conferenceId;
            ParticipantId = participantId;
            Fullname = fullname;
            DisplayName = displayName;
            Representee = representee;
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
            var conference = await _context.Conferences.Include("Participants")
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

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
            participant.DisplayName = command.DisplayName;
            participant.Representee = command.Representee;
            await _context.SaveChangesAsync();
        }
    }

}