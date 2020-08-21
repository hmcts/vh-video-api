using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class RemoveEndpointCommand : ICommand
    {
        public Guid ConferenceId { get; }
        public Guid EndpointId { get; }
        
        public RemoveEndpointCommand(Guid conferenceId, Guid endpointId)
        {
            ConferenceId = conferenceId;
            EndpointId = endpointId;
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

            var ep = conference.GetEndpoints().SingleOrDefault(x => x.Id == command.EndpointId);
            if (ep == null)
            {
                throw new EndpointNotFoundException(command.EndpointId);
            }

            conference.RemoveEndpoint(ep);
            await _context.SaveChangesAsync();
        }
    }
}
