using System;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class AddStaffMemberToConferenceCommand : ICommand
    {
        public Guid ConferenceId { get; set; }
        public ParticipantBase StaffMember { get; set; }

        public AddStaffMemberToConferenceCommand(Guid conferenceId, ParticipantBase staffMember)
        {
            ConferenceId = conferenceId;
            StaffMember = staffMember;
        }
    }

    public class AddStaffMemberToConferenceCommandHandler : ICommandHandler<AddStaffMemberToConferenceCommand>
    {
        private readonly VideoApiDbContext _context;

        public AddStaffMemberToConferenceCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(AddStaffMemberToConferenceCommand command)
        {
            var conference = await _context.Conferences.Include(x => x.Participants)
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }
            conference.AddParticipant(command.StaffMember);
            _context.Entry(command.StaffMember).State = EntityState.Added;
            await _context.SaveChangesAsync();
        }
    }
}
