using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class AddRoomParticipantCommand : ICommand
    {
        public long RoomId { get; }
        public RoomParticipant RoomParticipant { get; }

        public AddRoomParticipantCommand(long roomId, RoomParticipant participant)
        {
            RoomId = roomId;
            RoomParticipant = participant;
        }
    }

    public class AddRoomParticipantCommandHandler : ICommandHandler<AddRoomParticipantCommand>
    {
        private readonly VideoApiDbContext _context;

        public AddRoomParticipantCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddRoomParticipantCommand command)
        {
            var room = await _context.Rooms.Include(x=> x.RoomParticipants)
                .SingleOrDefaultAsync(x => x.Id == command.RoomId);

            if (room == null)
            {
                throw new RoomNotFoundException(command.RoomId);
            }

            room.AddParticipant(command.RoomParticipant);
            _context.Entry(command.RoomParticipant).State = EntityState.Added;

            await _context.SaveChangesAsync();

        }
    }
}
