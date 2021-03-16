using System;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class CreateInterpreterRoomCommand : ICommand
    {
        public Guid ConferenceId { get; }
        public VirtualCourtRoomType Type { get; }
        public long NewRoomId { get; internal set; }
        
        public CreateInterpreterRoomCommand(Guid conferenceId, VirtualCourtRoomType type)
        {
            ConferenceId = conferenceId;
            Type = type;
        }
    }
    
    public class CreateInterpreterRoomCommandHandler : ICommandHandler<CreateInterpreterRoomCommand>
    {
        private readonly VideoApiDbContext _context;

        public CreateInterpreterRoomCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(CreateInterpreterRoomCommand command)
        {
            var conference = await _context.Conferences.SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            var room = new InterpreterRoom(command.ConferenceId, command.Type);
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            command.NewRoomId = room.Id;
        }
    }
}
