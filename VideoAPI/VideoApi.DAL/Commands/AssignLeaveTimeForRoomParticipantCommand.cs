using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class AssignLeaveTimeForRoomParticipantCommand : ICommand
    {
        public long RoomId { get; set; }
        public Guid ParticipantId { get; set; }

        public DateTime LeaveTime { get; set; }

        public AssignLeaveTimeForRoomParticipantCommand(long roomId, Guid participantId, DateTime leaveTime)
        {
            RoomId = roomId;
            ParticipantId = participantId;
            LeaveTime = leaveTime;
        }
    }

    public class AssignLeaveTimeForRoomParticipantCommandHandler : ICommandHandler<AssignLeaveTimeForRoomParticipantCommand>
    {
        private readonly VideoApiDbContext _context;

        public AssignLeaveTimeForRoomParticipantCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AssignLeaveTimeForRoomParticipantCommand command)
        {
            var room = await _context.Rooms.Include("RoomParticipants").SingleOrDefaultAsync(x => x.Id == command.RoomId);

            if (room == null)
            {
                throw new RoomNotFoundException(command.RoomId);
            }

            var roomParticipant = room.RoomParticipants.SingleOrDefault(x => x.ParticipantId == command.ParticipantId);
            if (roomParticipant == null)
            {
                throw new RoomParticipantNotFoundException(command.ParticipantId, command.RoomId);
            }

            roomParticipant.AssignLeaveTime(command.LeaveTime);
            await _context.SaveChangesAsync();
        }
    }
}
