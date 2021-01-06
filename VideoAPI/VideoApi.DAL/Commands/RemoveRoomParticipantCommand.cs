using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class RemoveRoomParticipantCommand : ICommand
    {
        public long RoomId { get; }
        public Guid ParticipantId { get; }

        public RemoveRoomParticipantCommand(Guid participantId, long roomId)
        {
            ParticipantId = participantId;
            RoomId = roomId;
        }
    }

    public class RemoveRoomParticipantCommandHandle : ICommandHandler<RemoveRoomParticipantCommand>
    {
        private readonly VideoApiDbContext _context;

        public RemoveRoomParticipantCommandHandle(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(RemoveRoomParticipantCommand command)
        {
            var room = await _context.Rooms.Include("RoomParticipants").SingleOrDefaultAsync(x => x.Id == command.RoomId);

            if (room == null)
            {
                throw new RoomNotFoundException(command.RoomId);
            }

            var roomParticipant = room.RoomParticipants.FirstOrDefault(x => x.ParticipantId == command.ParticipantId);
            if (roomParticipant == null)
            {
                throw new RoomParticipantNotFoundException(command.ParticipantId, command.RoomId);
            }

            room.RemoveParticipant(roomParticipant);

            await _context.SaveChangesAsync();
        }
    }
}
