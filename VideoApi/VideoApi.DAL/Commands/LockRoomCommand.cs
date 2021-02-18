using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class LockRoomCommand : ICommand
    {
        public Guid ConferenceId { get; }
        public string Label { get; }
        public bool Locked { get; private set; }

        public LockRoomCommand(Guid conferenceId, string label, bool locked)
        {
            ConferenceId = conferenceId;
            Label = label;
            Locked = locked;
        }
    }

    public class LockRoomCommandHandler : ICommandHandler<LockRoomCommand>
    {
        private readonly VideoApiDbContext _context;

        public LockRoomCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(LockRoomCommand command)
        {
            var room = await _context.Rooms
                .Where(x => x.ConferenceId == command.ConferenceId && x.Label == command.Label)
                .Where(x => x.Status == RoomStatus.Live)
                .SingleOrDefaultAsync();

            if (room == null)
            {
                throw new RoomNotFoundException(command.ConferenceId, command.Label);
            }

            room.UpdateRoomLock(command.Locked);
            await _context.SaveChangesAsync();
        }
    }
}
