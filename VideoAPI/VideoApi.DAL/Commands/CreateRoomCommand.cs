using Microsoft.EntityFrameworkCore;
using System;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class CreateRoomCommand : ICommand
    {

        public Guid ConferenceId { get; }
        public string Label { get; }
        public VirtualCourtRoomType Type { get; }
        public long NewRoomId { get; set; }

        public CreateRoomCommand(Guid conferenceId, string label, VirtualCourtRoomType type)
        {
            ConferenceId = conferenceId;
            Label = label;
            Type = type;
        }
    }

    public class CreateRoomCommandHandler : ICommandHandler<CreateRoomCommand>
    {
        private readonly VideoApiDbContext _context;

        public CreateRoomCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(CreateRoomCommand command)
        {
            var conference = await _context.Conferences.SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            var room = new Room(command.ConferenceId, command.Label, command.Type);

            _context.Rooms.Add(room);

            await _context.SaveChangesAsync();

            command.NewRoomId = room.Id;
        }
    }
}
