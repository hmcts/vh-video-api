using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Commands
{
    public class UpdateEndpointStatusAndRoomCommand : ICommand
    {
        public Guid ConferenceId { get; }
        public Guid EndpointId { get; }
        public EndpointState Status { get; }
        public RoomType? Room { get; }

        public UpdateEndpointStatusAndRoomCommand(Guid conferenceId, Guid endpointId, EndpointState status,
            RoomType? room)
        {
            ConferenceId = conferenceId;
            EndpointId = endpointId;
            Status = status;
            Room = room;
        }
    }

    public class UpdateEndpointStatusAndRoomCommandHandler : ICommandHandler<UpdateEndpointStatusAndRoomCommand>
    {
        private readonly VideoApiDbContext _context;

        public UpdateEndpointStatusAndRoomCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateEndpointStatusAndRoomCommand command)
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
            endpoint.UpdateCurrentRoom(command.Room);

            await _context.SaveChangesAsync();
        }
    }
}
