using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Commands
{
    public class UpdateEndpointStatusCommand : ICommand
    {
        public Guid ConferenceId { get; set; }
        public Guid EndpointId { get; set; }
        public EndpointState Status { get; set; }
        public UpdateEndpointStatusCommand(Guid conferenceId, Guid endpointId, EndpointState status)
        {
            ConferenceId = conferenceId;
            EndpointId = endpointId;
            Status = status;
        }
    }

    public class UpdateEndpointStatusCommandHandler : ICommandHandler<UpdateEndpointStatusCommand>
    {
        private readonly VideoApiDbContext _context;

        public UpdateEndpointStatusCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateEndpointStatusCommand command)
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
            
            endpoint.UpdateStatus(command.Status);
            await _context.SaveChangesAsync();
        }
    }
}
