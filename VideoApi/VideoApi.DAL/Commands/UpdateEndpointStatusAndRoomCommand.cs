using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class UpdateEndpointStatusAndRoomCommand : ICommand
    {
        public Guid ConferenceId { get; }
        public Guid EndpointId { get; }
        public EndpointState Status { get; }
        public RoomType? Room { get; }
        public string RoomLabel { get; }

        public UpdateEndpointStatusAndRoomCommand(
            Guid conferenceId,
            Guid endpointId,
            EndpointState status,
            RoomType? room,
            string roomLabel)
        {
            ConferenceId = conferenceId;
            EndpointId = endpointId;
            Status = status;
            Room = room;
            RoomLabel = roomLabel;
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
            var conference = await _context.Conferences
                .Include(x => x.Endpoints)
                .ThenInclude(x => x.CurrentVirtualRoom).ThenInclude(x => x.RoomEndpoints)
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

            Room virtualRoom = null;
            if (command.Status == EndpointState.InConsultation)
            {
                var virtualRooms = await _context.Rooms
                    .Where(x => x.Label == command.RoomLabel && x.ConferenceId == command.ConferenceId).ToListAsync();
                virtualRoom = virtualRooms.LastOrDefault();
                if (!command.Room.HasValue && virtualRoom == null && command.Status != EndpointState.Disconnected)
                {
                    throw new RoomNotFoundException(command.RoomLabel);
                }
            }

            UpdateRoom(command.Status, virtualRoom, endpoint);
            endpoint.UpdateStatus(command.Status);
            endpoint.UpdateCurrentRoom(command.Room);
            endpoint.UpdateCurrentVirtualRoom(virtualRoom);

            await _context.SaveChangesAsync();
        }

        private void UpdateRoom(EndpointState status, Room virtualRoom, Endpoint endpoint)
        {
            var isDynamicConsultationRoom = virtualRoom != null;
            if (status == EndpointState.InConsultation && isDynamicConsultationRoom)
            {
                virtualRoom.AddEndpoint(new RoomEndpoint(endpoint.Id));
            }

            if (status != EndpointState.InConsultation)
            {
                endpoint.CurrentVirtualRoom?.RemoveEndpoint(new RoomEndpoint(endpoint.Id));
            }
        }
    }
}
