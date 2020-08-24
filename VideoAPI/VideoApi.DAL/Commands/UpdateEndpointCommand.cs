using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;

namespace VideoApi.DAL.Commands
{
    public class UpdateEndpointCommand : ICommand
    {
        public Guid ConferenceId { get; set; }
        public Guid EndpointId { get; set; }
        public string DisplayName { get; set; }
        public UpdateEndpointCommand(Guid conferenceId, Guid endpointId, string displayName)
        {
            ConferenceId = conferenceId;
            EndpointId = endpointId;
            DisplayName = displayName;
        }
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
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            var endpoint = conference.GetEndpoints().SingleOrDefault(x => x.Id == command.EndpointId);
            if (endpoint == null)
            {
                throw new EndpointNotFoundException(command.EndpointId);
            }

            endpoint.UpdateDisplayName(command.DisplayName);
            await _context.SaveChangesAsync();
        }
    }
}
