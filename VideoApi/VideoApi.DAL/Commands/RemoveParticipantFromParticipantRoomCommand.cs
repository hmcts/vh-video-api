using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
   
    public class RemoveParticipantFromParticipantRoomCommand : ICommand
    {
        public long RoomId { get; }
        public Guid ParticipantId { get; }
        
        public RemoveParticipantFromParticipantRoomCommand(long roomId, Guid participantId)
        {
            RoomId = roomId;
            ParticipantId = participantId;
        }
    }

    public class RemoveParticipantFromParticipantRoomCommandHandler : ICommandHandler<RemoveParticipantFromParticipantRoomCommand>
    {
        private readonly VideoApiDbContext _context;

        public RemoveParticipantFromParticipantRoomCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(RemoveParticipantFromParticipantRoomCommand command)
        {
            var room = await _context.Rooms.OfType<ParticipantRoom>().Include(x => x.RoomParticipants)
                .SingleOrDefaultAsync(x => x.Id == command.RoomId);

            if (room == null)
            {
                throw new RoomNotFoundException(command.RoomId);
            }
            
            room.RemoveParticipant(new RoomParticipant(command.ParticipantId));
            await _context.SaveChangesAsync();
        }
    }
}
