using System;
using Microsoft.EntityFrameworkCore;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Exceptions;
using VideoApi.Domain;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.DAL.Commands;

public class AddTelephoneParticipantCommand : ICommand
{
    public AddTelephoneParticipantCommand(Guid conferenceId, Guid telephoneParticipantId, string telephoneNumber)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(telephoneNumber);
        ConferenceId = conferenceId;
        TelephoneParticipantId = telephoneParticipantId;
        TelephoneNumber = telephoneNumber;
    }

    public Guid TelephoneParticipantId { get; set; }
    public Guid ConferenceId { get; }
    public string TelephoneNumber { get; }
    
}

public class AddTelephoneParticipantCommandHandler(VideoApiDbContext context)
    : ICommandHandler<AddTelephoneParticipantCommand>
{
    public async Task Handle(AddTelephoneParticipantCommand command)
    {
        var conference = await context.Conferences.Include(x => x.TelephoneParticipants)
            .SingleOrDefaultAsync(x => x.Id == command.ConferenceId);

        if (conference == null)
        {
            throw new ConferenceNotFoundException(command.ConferenceId);
        }

        var telephoneParticipant = new TelephoneParticipant(command.TelephoneParticipantId, command.TelephoneNumber);
        conference.AddTelephoneParticipant(telephoneParticipant);
        await context.SaveChangesAsync();
    }
}
