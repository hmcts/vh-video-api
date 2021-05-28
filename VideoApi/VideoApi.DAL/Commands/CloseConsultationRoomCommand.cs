using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Commands
{
    public class CloseConsultationRoomCommand : ICommand
    {
        public long RoomId { get; }

        public CloseConsultationRoomCommand(long roomId)
        {
            RoomId = roomId;
        }
    }
    
    public class CloseConsultationRoomCommandHandler : ICommandHandler<CloseConsultationRoomCommand>
    {
        private readonly VideoApiDbContext _context;

        public CloseConsultationRoomCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(CloseConsultationRoomCommand command)
        {
            var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == command.RoomId);
            
            if (room == null)
                throw new RoomNotFoundException(command.RoomId);

            if (room.Type != VirtualCourtRoomType.Participant && room.Type != VirtualCourtRoomType.JudgeJOH)
                throw new InvalidVirtualCourtRoomTypeException(room.Type, "CloseConsultationRoomCommandHandler");

            room.CloseRoom();
            
            await _context.SaveChangesAsync();
        }
    }
}
