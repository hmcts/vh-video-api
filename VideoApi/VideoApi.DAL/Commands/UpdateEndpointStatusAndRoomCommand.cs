using System;
using System.Linq;
using System.Threading.Tasks;
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
                .Include(x => x.Endpoints).ThenInclude(x => x.CurrentConsultationRoom).ThenInclude(x => x.RoomEndpoints)
                .Include(x => x.Endpoints).ThenInclude(x => x.CurrentConsultationRoom).ThenInclude(x => x.RoomParticipants)
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

            var transferToRoom = await GetTransferToRoom(command).ConfigureAwait(true);
            transferToRoom?.AddEndpoint(new RoomEndpoint(endpoint.Id));

            endpoint.UpdateStatus(command.Status);
            endpoint.UpdateCurrentRoom(command.Room);
            endpoint.UpdateCurrentVirtualRoom(transferToRoom);
            await _context.SaveChangesAsync();
        }

        private async Task<ConsultationRoom> GetTransferToRoom(UpdateEndpointStatusAndRoomCommand command)
        {
            if (command.Status != EndpointState.InConsultation)
            {
                return null;
            }

            var transferToRoom = await _context.Rooms.OfType<ConsultationRoom>()
                .SingleOrDefaultAsync(x => x.Label == command.RoomLabel && x.ConferenceId == command.ConferenceId)
                .ConfigureAwait(true);
            if (transferToRoom == null)
            {
                // The only way for the room not to have been created by us (where it would already be in the table) is by kinly via a VHO consultation.
                var vhoConsultation = new ConsultationRoom(command.ConferenceId, command.RoomLabel, VirtualCourtRoomType.Participant, false);
                _context.Rooms.Add(vhoConsultation);
                transferToRoom = vhoConsultation;
            }

            return transferToRoom;
        }
    }
}
