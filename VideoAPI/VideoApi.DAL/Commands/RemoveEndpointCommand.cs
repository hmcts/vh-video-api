using System;
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
        public string EndpointAddress { get; }
        
        public RemoveEndpointCommand(Guid conferenceId, string endpointAddress)
        {
            ConferenceId = conferenceId;
            EndpointAddress = endpointAddress;
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

            conference.RemoveEndpoint(new Endpoint(string.Empty, command.EndpointAddress, string.Empty));
            await _context.SaveChangesAsync();
        }
    }
}
