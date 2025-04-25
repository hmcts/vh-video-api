using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VideoApi.Common;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using VideoApi.Common.Logging;

namespace VideoApi.Events.Handlers;

public class TelephoneTransferredEventHandler(
    IQueryHandler queryHandler,
    ICommandHandler commandHandler,
    ILogger<TelephoneTransferredEventHandler> logger)
    : EventHandlerBase<TelephoneTransferredEventHandler>(queryHandler, commandHandler, logger)
{
    public override EventType EventType => EventType.TelephoneTransfer;

    protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
    {
        BadRequestException.ThrowIfNull(callbackEvent.TransferTo);
        Logger.LogTelephoneTransferredCallback(SourceConference.Id, SourceTelephoneParticipant.Id, callbackEvent.TransferredFromRoomLabel, callbackEvent.TransferredToRoomLabel);
        var room = callbackEvent.TransferTo;
        var state =  room.HasValue ? TelephoneState.Connected : TelephoneState.Disconnected;
        var command = new UpdateTelephoneParticipantCommand(SourceConference.Id, SourceTelephoneParticipant.Id, room, state);
        await CommandHandler.Handle(command);
    }
}
