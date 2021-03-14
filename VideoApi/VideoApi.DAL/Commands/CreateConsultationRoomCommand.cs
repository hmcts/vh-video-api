using Microsoft.EntityFrameworkCore;
using System;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class CreateConsultationRoomCommand : ICommand
    {
        public Guid ConferenceId { get; }
        public string Label { get; }
        public VirtualCourtRoomType Type { get; }
        public long NewRoomId { get; private set; }
        public bool Locked { get; private set; }

        public CreateConsultationRoomCommand(Guid conferenceId, string label, VirtualCourtRoomType type, bool locked)
        {
            ConferenceId = conferenceId;
            Label = label;
            Type = type;
            Locked = locked;
        }

        public void UpdateNewRoomId(long newRoomId)
        {
            NewRoomId = newRoomId;
        }
    }

    public class CreateConsultationRoomCommandHandler : ICommandHandler<CreateConsultationRoomCommand>
    {
        private readonly VideoApiDbContext _context;

        public CreateConsultationRoomCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(CreateConsultationRoomCommand command)
        {
            var conference = await _context.Conferences.SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            var room = string.IsNullOrWhiteSpace(command.Label)
                ? new ConsultationRoom(command.ConferenceId, command.Type, command.Locked)
                : new ConsultationRoom(command.ConferenceId, command.Label, command.Type, command.Locked);
            

            _context.Rooms.Add(room);

            await _context.SaveChangesAsync();

            command.UpdateNewRoomId(room.Id);
        }
    }
}
