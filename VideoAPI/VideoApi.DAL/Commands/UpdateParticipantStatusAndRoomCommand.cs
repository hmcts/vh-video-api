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
    public class UpdateParticipantStatusAndRoomCommand : ICommand
    {
        public Guid ConferenceId { get; }
        public Guid ParticipantId { get; }
        public ParticipantState ParticipantState { get; }
        public RoomType? Room { get; }
        public string RoomLabel { get; }

        public UpdateParticipantStatusAndRoomCommand(Guid conferenceId, Guid participantId,
            ParticipantState participantState, RoomType? room, string roomLabel)
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
            var conference = await _context.Conferences.Include(x => x.Participants)
                .ThenInclude(x => x.CurrentVirtualRoom).ThenInclude(x => x.RoomParticipants)
                .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

            if (conference == null)
            {
                throw new ConferenceNotFoundException(command.ConferenceId);
            }
            
            var virtualRoom = await _context.Rooms.SingleOrDefaultAsync(x => x.Label == command.RoomLabel);

            if (!command.Room.HasValue && virtualRoom == null && command.ParticipantState != ParticipantState.Disconnected)
            {
                throw new RoomNotFoundException(command.RoomLabel);
            }
            
            var participant = conference.GetParticipants().SingleOrDefault(x => x.Id == command.ParticipantId);
            if (participant == null)
            {
                throw new ParticipantNotFoundException(command.ParticipantId);
            }

            UpdateRoom(command.ParticipantState, virtualRoom, participant);
            participant.UpdateParticipantStatus(command.ParticipantState);
            participant.UpdateCurrentRoom(command.Room);
            participant.UpdateCurrentVirtualRoom(virtualRoom);
            await _context.SaveChangesAsync();
        }

        private void UpdateRoom(ParticipantState status, Room vRoom, Participant participant)
        {
            var isDynamicConsultationRoom = vRoom != null;
            if (status == ParticipantState.InConsultation && isDynamicConsultationRoom)
            {
                vRoom.AddParticipant(new RoomParticipant(participant.Id));
            }

            if (status != ParticipantState.InConsultation && participant.CurrentVirtualRoom != null)
            {
                participant.CurrentVirtualRoom.RemoveParticipant(new RoomParticipant(participant.Id));
            }
        }
    }
}
