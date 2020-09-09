using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class RemoveEndpointCommand : ICommand
    {
        public Guid ConferenceId { get; }
        public string SipAddress { get; }
        
        public RemoveEndpointCommand(Guid conferenceId, string sipAddress)
        {
            ConferenceId = conferenceId;
            SipAddress = sipAddress;
        }
    }

    public class RemoveEndpointCommandHandler : ICommandHandler<RemoveEndpointCommand>
    {
        private readonly VideoApiDbContext _context;

        public RemoveEndpointCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(RemoveEndpointCommand command)
        {
            var conference = await _context.Conferences.Include(x => x.Endpoints)
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            var ep = conference.GetEndpoints().SingleOrDefault(x => x.SipAddress == command.SipAddress);
            if (ep == null)
            {
                throw new EndpointNotFoundException(command.SipAddress);
            }

            conference.RemoveEndpoint(ep);
            await _context.SaveChangesAsync();
        }
    }
}
