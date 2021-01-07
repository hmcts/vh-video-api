using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class UpdateRoomStatusCommand : ICommand
    {
        public long RoomId { get; }
        public RoomStatus Status { get; }

        public UpdateRoomStatusCommand(long roomId, RoomStatus status)
        {
            RoomId = roomId;
            Status = status;
        }
    }

    public class UpdateRoomStatusCommandHandler : ICommandHandler<UpdateRoomStatusCommand>
    {
        private readonly VideoApiDbContext _context;

        public UpdateRoomStatusCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateRoomStatusCommand command)
        {
            var room = await _context.Rooms.SingleOrDefaultAsync(x => x.Id == command.RoomId);

            if (room == null)
            {
                throw new RoomNotFoundException(command.RoomId);
            }

            room.UpdateStatus(command.Status);
            await _context.SaveChangesAsync();
        }
    }
}
