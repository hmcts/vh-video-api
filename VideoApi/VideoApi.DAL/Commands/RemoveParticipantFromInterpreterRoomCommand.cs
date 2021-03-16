using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
   
    public class RemoveParticipantFromInterpreterRoomCommand : ICommand
    {
        public long RoomId { get; }
        public Guid ParticipantId { get; }
        
        public RemoveParticipantFromInterpreterRoomCommand(long roomId, Guid participantId)
        {
            RoomId = roomId;
            ParticipantId = participantId;
        }
    }

    public class RemoveParticipantFromInterpreterRoomCommandHandler : ICommandHandler<RemoveParticipantFromInterpreterRoomCommand>
    {
        private readonly VideoApiDbContext _context;

        public RemoveParticipantFromInterpreterRoomCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(RemoveParticipantFromInterpreterRoomCommand command)
        {
            var room = await _context.Rooms.OfType<InterpreterRoom>().Include(x => x.RoomParticipants)
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
