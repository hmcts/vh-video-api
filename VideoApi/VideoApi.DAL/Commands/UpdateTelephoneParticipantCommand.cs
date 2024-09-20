using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain.Enums;

namespace VideoApi.DAL.Commands;

public class UpdateTelephoneParticipantCommand : ICommand
{
    public UpdateTelephoneParticipantCommand(Guid conferenceId, Guid telephoneParticipantId, RoomType? room, TelephoneState state)
    {
        ConferenceId = conferenceId;
        TelephoneParticipantId = telephoneParticipantId;
        Room = room;
        State = state;
    }

    public Guid ConferenceId { get; }
    public Guid TelephoneParticipantId { get; }
    public RoomType? Room { get; }
    public TelephoneState State { get; }
}

public class UpdateTelephoneParticipantCommandHandler(VideoApiDbContext context) : ICommandHandler<UpdateTelephoneParticipantCommand>
{
    public async Task Handle(UpdateTelephoneParticipantCommand command)
    {
        var conference = await context.Conferences.Include(x => x.TelephoneParticipants)
            .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

        if (conference == null)
        {
            throw new ConferenceNotFoundException(command.ConferenceId);
        }

        var telephoneParticipant = conference.GetTelephoneParticipants().SingleOrDefault(x => x.Id == command.TelephoneParticipantId);
        if (telephoneParticipant == null)
        {
            throw new TelephoneParticipantNotFoundException(command.TelephoneParticipantId);
        }

        telephoneParticipant.UpdateCurrentRoom(command.Room);
        telephoneParticipant.UpdateStatus(command.State);
        
        await context.SaveChangesAsync();
    }
}
