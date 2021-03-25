using System;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class CreateParticipantRoomCommand : ICommand
    {
        public Guid ConferenceId { get; }
        public VirtualCourtRoomType Type { get; }
        public long NewRoomId { get; internal set; }
        
        public CreateParticipantRoomCommand(Guid conferenceId, VirtualCourtRoomType type)
        {
            ConferenceId = conferenceId;
            Type = type;
        }
    }
    
    public class CreateParticipantRoomCommandHandler : ICommandHandler<CreateParticipantRoomCommand>
    {
        private readonly VideoApiDbContext _context;

        public CreateParticipantRoomCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(CreateParticipantRoomCommand command)
        {
            var conference = await _context.Conferences.SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            var room = new ParticipantRoom(command.ConferenceId, command.Type);
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            command.NewRoomId = room.Id;
        }
    }
}
