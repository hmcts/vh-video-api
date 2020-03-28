using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Commands
{
    public class UpdateParticipantStatusAndRoomCommand : ICommand
    {
        public Guid ConferenceId { get; }
        public Guid ParticipantId { get; }
        public ParticipantState ParticipantState { get; }
        public RoomType? Room { get; }

        public UpdateParticipantStatusAndRoomCommand(Guid conferenceId, Guid participantId,
            ParticipantState participantState, RoomType? room)
        {
            ConferenceId = conferenceId;
            ParticipantId = participantId;
            ParticipantState = participantState;
            Room = room;
        }
    }
    
    public class UpdateParticipantStatusAndRoomCommandHandler : ICommandHandler<UpdateParticipantStatusAndRoomCommand>
    {
        private readonly VideoApiDbContext _context;
        
        public UpdateParticipantStatusAndRoomCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }
        
        public async Task Handle(UpdateParticipantStatusAndRoomCommand command)
        {
            var conference = await _context.Conferences.Include(x => x.Participants)
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
            participant.UpdateCurrentRoom(command.Room);
            
            await _context.SaveChangesAsync();
        }
    }
}
