using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;

namespace VideoApi.Events.Handlers;

public class TelephoneDisconnectedEventHandler(
    IQueryHandler queryHandler,
    ICommandHandler commandHandler,
    ILogger<TelephoneDisconnectedEventHandler> logger)
    : EventHandlerBase<TelephoneDisconnectedEventHandler>(queryHandler, commandHandler, logger)
{
    public override EventType EventType => EventType.TelephoneDisconnected;

    protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
    {
        Logger.LogInformation("TelephoneDisconnected callback - {ConferenceId}/{TelephoneParticipantId}",
            SourceConference.Id, SourceTelephoneParticipant.Id);
        var command = new RemoveTelephoneParticipantCommand(SourceConference.Id, SourceTelephoneParticipant.Id);
        await CommandHandler.Handle(command);
    }
}
