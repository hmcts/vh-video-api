using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;

namespace VideoApi.DAL.Commands;

public class RemoveTelephoneParticipantCommand : ICommand
{
    public RemoveTelephoneParticipantCommand(Guid conferenceId, Guid telephoneParticipantId)
    {
        ConferenceId = conferenceId;
        TelephoneParticipantId = telephoneParticipantId;
    }

    public Guid ConferenceId { get; }
    public Guid TelephoneParticipantId { get; }
}

public class RemoveTelephoneParticipantCommandHandler(VideoApiDbContext context)
    : ICommandHandler<RemoveTelephoneParticipantCommand>
{
    public async Task Handle(RemoveTelephoneParticipantCommand command)
    {
        var conference = await context.Conferences.SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

        if (conference == null)
        {
            throw new ConferenceNotFoundException(command.ConferenceId);
        }

        var telephoneParticipant = conference.GetTelephoneParticipants().SingleOrDefault(x => x.Id == command.TelephoneParticipantId);
        if (telephoneParticipant == null)
        {
            throw new TelephoneParticipantNotFoundException(command.TelephoneParticipantId);
        }

        conference.RemoveTelephoneParticipant(telephoneParticipant);
        await context.SaveChangesAsync();
    }
}
