using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Commands
{
    public class UpdateParticipantStatusCommand : ICommand
    {
        public Guid ConferenceId { get; }
        public long ParticipantId { get; }
        public ParticipantState ParticipantState { get; }

        public UpdateParticipantStatusCommand(Guid conferenceId, long participantId, ParticipantState participantState)
        {
            ConferenceId = conferenceId;
            ParticipantId = participantId;
            ParticipantState = participantState;
        }
    }

    public class UpdateParticipantStatusCommandHandler : ICommandHandler<UpdateParticipantStatusCommand>
    {
        private readonly VideoApiDbContext _context;
        
        public UpdateParticipantStatusCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }
        
        public async Task Handle(UpdateParticipantStatusCommand command)
        {
            var conference = await _context.Conferences.Include("Participants.ParticipantStatuses")
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
            
            participant.UpdateParticipantStatus(command.ParticipantState);
            await _context.SaveChangesAsync();
        }
    }
}