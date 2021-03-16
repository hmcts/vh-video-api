using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class AddParticipantToInterpreterRoomCommand : ICommand
    {
        public long RoomId { get; }
        public Guid ParticipantId { get; }

        public AddParticipantToInterpreterRoomCommand(long roomId, Guid participantId)
        {
            RoomId = roomId;
            ParticipantId = participantId;
        }
    }

    public class AddParticipantToInterpreterRoomCommandHandler : ICommandHandler<AddParticipantToInterpreterRoomCommand>
    {
        private readonly VideoApiDbContext _context;

        public AddParticipantToInterpreterRoomCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddParticipantToInterpreterRoomCommand command)
        {
            var room = await _context.Rooms.OfType<InterpreterRoom>().SingleOrDefaultAsync(x => x.Id == command.RoomId);

            if (room == null)
            {
                throw new RoomNotFoundException(command.RoomId);
            }

            room.AddParticipant(new RoomParticipant(command.ParticipantId));
            await _context.SaveChangesAsync();
        }
    }
}
