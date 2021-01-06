using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Commands
{
    public class CreateRoomCommand : ICommand
    {
        public Guid RoomId { get; set; }
        public Guid ConferenceId { get; set; }
        public Guid RequestedBy { get; set; }
        public RoomType Room { get; set; }
        public string RoomStatus { get; set; }
        
        public CreateRoomCommand(Guid conferenceId, Guid participantId, RoomType room)
        {
            ConferenceId = conferenceId;
            RequestedBy = participantId;
            Room = room;
        }
    }

    public class CreateRoomCommandHandler : ICommandHandler<CreateRoomCommand>
    {
        private readonly VideoApiDbContext _context;

        public CreateRoomCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }
        
        public async Task Handle(CreateRoomCommand command)
        {
            var conference = await _context.Conferences.Include(x => x.Participants)
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);
            
            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            var participant = conference.GetParticipants().SingleOrDefault(x => x.Id == command.RequestedBy);
            if (participant == null)
            {
                throw new ParticipantNotFoundException(command.RequestedBy);
            }

            command.RoomId = Guid.NewGuid();
            command.ConferenceId = conference.Id;
            command.RequestedBy = participant.Id;
            
            //TODO: Update to enum when created in domain
            command.RoomStatus = "RoomStatus.Created;";

            await _context.SaveChangesAsync();
        }
    }
}
