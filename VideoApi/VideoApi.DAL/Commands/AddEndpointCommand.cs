using System;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class AddEndpointCommand(
        Guid conferenceId,
        string displayName,
        string sipAddress,
        string pin,
        ConferenceRole conferenceRole)
        : ICommand
    {
        public Guid ConferenceId { get; } = conferenceId;
        public string DisplayName { get; } = displayName;
        public string SipAddress { get; } = sipAddress;
        public string Pin { get; } = pin;
        public ConferenceRole ConferenceRole { get; } = conferenceRole;
    }

    public class AddEndpointCommandHandler : ICommandHandler<AddEndpointCommand>
    {
        private readonly VideoApiDbContext _context;
        
        public AddEndpointCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }
        
        public async Task Handle(AddEndpointCommand command)
        {
            var conference = await _context.Conferences.Include(x => x.Endpoints)
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            var ep = new Endpoint(command.DisplayName, command.SipAddress, command.Pin, command.ConferenceRole);
            conference.AddEndpoint(ep);
            _context.Entry(ep).State = EntityState.Added;
            await _context.SaveChangesAsync();
        }
    }
}
