using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using VideoApi.Services;
using VideoApi.Common.Logging;

namespace VideoApi.Events.Handlers;

public class TelephoneJoinedEventHandler(
    IQueryHandler queryHandler,
    ICommandHandler commandHandler,
    ILogger<TelephoneJoinedEventHandler> logger,
    ISupplierPlatformServiceFactory supplierPlatformServiceFactory)
    : EventHandlerBase<TelephoneJoinedEventHandler>(queryHandler, commandHandler, logger)
{
    public override EventType EventType => EventType.TelephoneJoined;

    protected override async Task PublishStatusAsync(CallbackEvent callbackEvent)
    {
        Logger.LogTelephoneJoinedCallback(SourceConference.Id, callbackEvent.ParticipantId);
        ValidateTelephoneParticipantEventReceivedAfterLastUpdate(callbackEvent);
        
        var command =
            new AddTelephoneParticipantCommand(SourceConference.Id, callbackEvent.ParticipantId, callbackEvent.Phone);

        TransferToHearingRoomIfHearingIsAlreadyInSession(callbackEvent.ParticipantId);

        await CommandHandler.Handle(command);
    }

    private void TransferToHearingRoomIfHearingIsAlreadyInSession(Guid telephoneParticipantId)
    {
        Logger.LogConferenceState(SourceConference.Id, SourceConference.State.ToString());
        if (SourceConference.State != ConferenceState.InSession) return;
            
        Logger.LogTransferringTelephoneParticipantToHearingRoom(SourceConference.Id, telephoneParticipantId);
        var videoPlatformService = supplierPlatformServiceFactory.Create(SourceConference.Supplier);
        videoPlatformService.TransferParticipantAsync(SourceConference.Id, telephoneParticipantId.ToString(),
            RoomType.WaitingRoom.ToString(), RoomType.HearingRoom.ToString(), ConferenceRole.Guest);
    }
}
