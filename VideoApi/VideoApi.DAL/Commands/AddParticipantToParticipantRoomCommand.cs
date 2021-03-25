using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class AddParticipantToParticipantRoomCommand : ICommand
    {
        public long RoomId { get; }
        public Guid ParticipantId { get; }

        public AddParticipantToParticipantRoomCommand(long roomId, Guid participantId)
        {
            RoomId = roomId;
            ParticipantId = participantId;
        }
    }

    public class AddParticipantToParticipantRoomCommandHandler : ICommandHandler<AddParticipantToParticipantRoomCommand>
    {
        private readonly VideoApiDbContext _context;

        public AddParticipantToParticipantRoomCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddParticipantToParticipantRoomCommand command)
        {
            var room = await _context.Rooms.OfType<ParticipantRoom>().SingleOrDefaultAsync(x => x.Id == command.RoomId);

            if (room == null)
            {
                throw new RoomNotFoundException(command.RoomId);
            }

            room.AddParticipant(new RoomParticipant(command.ParticipantId));
            await _context.SaveChangesAsync();
        }
    }
}
