using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands
{
    public class UpdateParticipantStatusAndRoomCommand : ICommand
    {
        public Guid ConferenceId { get; }
        public Guid ParticipantId { get; }
        public ParticipantState ParticipantState { get; }
        public RoomType? Room { get; }
        public string RoomLabel { get; }

        public UpdateParticipantStatusAndRoomCommand(
            Guid conferenceId,
            Guid participantId,
            ParticipantState participantState,
            RoomType? room,
            string roomLabel)
        {
            ConferenceId = conferenceId;
            ParticipantId = participantId;
            ParticipantState = participantState;
            Room = room;
            RoomLabel = roomLabel;
        }
    }
    
    public class UpdateParticipantStatusAndRoomCommandHandler : ICommandHandler<UpdateParticipantStatusAndRoomCommand>
    {
        private readonly VideoApiDbContext _context;

        public UpdateParticipantStatusAndRoomCommandHandler(VideoApiDbContext context)
        {
            _context = context;
        }
        
        public async Task Handle(UpdateParticipantStatusAndRoomCommand command)
        {
            var conference = await _context.Conferences
                .Include(x => x.Participants)
                .Include(x=> x.Rooms).ThenInclude(x=> x.RoomEndpoints)
                .Include(x=> x.Rooms).ThenInclude(x=> x.RoomParticipants)
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }

            var participant = conference.GetParticipants().SingleOrDefault(x => x.Id == command.ParticipantId);
            if (participant == null)
            {
                throw new ParticipantNotFoundException(command.ParticipantId);
            }

            var transferToRoom = await GetTransferToConsultationRoom(command).ConfigureAwait(true);
            transferToRoom?.AddParticipant(new RoomParticipant(participant.Id));
            
            participant.UpdateParticipantStatus(command.ParticipantState);
            participant.UpdateCurrentRoom(command.Room);
            participant.UpdateCurrentConsultationRoom(transferToRoom);
            await _context.SaveChangesAsync();
        }

        private async Task<ConsultationRoom> GetTransferToConsultationRoom(UpdateParticipantStatusAndRoomCommand command)
        {
            if (command.ParticipantState != ParticipantState.InConsultation)
            {
                return null;
            }

            var transferToRoom = await _context.Rooms.OfType<ConsultationRoom>().SingleOrDefaultAsync(x => x.Label == command.RoomLabel && x.ConferenceId == command.ConferenceId).ConfigureAwait(true);
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
