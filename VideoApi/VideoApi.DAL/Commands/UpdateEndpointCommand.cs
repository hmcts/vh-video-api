using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Commands
{
    public class UpdateEndpointCommand : ICommand
    {
        public UpdateEndpointCommand(Guid conferenceId, string sipAddress, string displayName, ConferenceRole conferenceRole)
        {
            ConferenceId = conferenceId;
            SipAddress = sipAddress;
            DisplayName = displayName;
            ConferenceRole = conferenceRole;
        }
        
        public Guid ConferenceId { get; }
        public string SipAddress { get; }
        public string DisplayName { get; }
        public ConferenceRole ConferenceRole { get; }
    }

    public class UpdateEndpointCommandHandler : ICommandHandler<UpdateEndpointCommand>
    {
        private readonly VideoApiDbContext _context;
        
        public UpdateEndpointCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }
        
        public async Task Handle(UpdateEndpointCommand command)
        {
            var conference = await _context.Conferences.Include(x => x.Endpoints)
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
                throw new ConferenceNotFoundException(command.ConferenceId);
            
            var endpoint = conference.GetEndpoints().SingleOrDefault(x => x.SipAddress == command.SipAddress);
            if (endpoint == null)
                throw new EndpointNotFoundException(command.SipAddress);

            if (!string.IsNullOrWhiteSpace(command.DisplayName)) 
                endpoint.UpdateDisplayName(command.DisplayName);
            
            endpoint.UpdateConferenceRole(command.ConferenceRole);
            await _context.SaveChangesAsync();
        }
    }
}
