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

            Room virtualRoom = null;
            if (command.ParticipantState == ParticipantState.InConsultation)
            {
                var virtualRooms = await _context.Rooms
                    .Where(x => x.Label == command.RoomLabel && x.ConferenceId == command.ConferenceId).ToListAsync();
                virtualRoom = virtualRooms.LastOrDefault();
                if (!command.Room.HasValue && virtualRoom == null && command.ParticipantState != ParticipantState.Disconnected)
                {
                    var vhoConsultation = new Room(command.ConferenceId, command.RoomLabel, VirtualCourtRoomType.Participant, false);
                    _context.Rooms.Add(vhoConsultation);
                    virtualRoom = vhoConsultation;
                }
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

        private void UpdateRoom(ParticipantState status, Room virtualRoom, Participant participant)
        {
            var isDynamicConsultationRoom = virtualRoom != null;
            if (status == ParticipantState.InConsultation && isDynamicConsultationRoom)
            {
                virtualRoom.AddParticipant(new RoomParticipant(participant.Id));
            }

            if (status != ParticipantState.InConsultation)
            {
                participant.CurrentVirtualRoom?.RemoveParticipant(new RoomParticipant(participant.Id));
            }
        }
    }
}
