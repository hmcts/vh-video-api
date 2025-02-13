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
        Logger.LogInformation("TelephoneJoined callback - {ConferenceId}/{TelephoneParticipantId}",
            SourceConference.Id, callbackEvent.ParticipantId);
        ValidateTelephoneParticipantEventReceivedAfterLastUpdate(callbackEvent);
        
        var command =
            new AddTelephoneParticipantCommand(SourceConference.Id, callbackEvent.ParticipantId, callbackEvent.Phone);

        TransferToHearingRoomIfHearingIsAlreadyInSession(callbackEvent.ParticipantId);

        await CommandHandler.Handle(command);
    }

    private void TransferToHearingRoomIfHearingIsAlreadyInSession(Guid telephoneParticipantId)
    {
        Logger.LogInformation("Conference {ConferenceId} state is {ConferenceState}", SourceConference.Id,
            SourceConference.State.ToString());
        if (SourceConference.State != ConferenceState.InSession) return;
            
        Logger.LogInformation("Conference {ConferenceId} already in session, transferring telephone participant {TelephoneParticipantId} to hearing room",
            SourceConference.Id, telephoneParticipantId);
        var videoPlatformService = supplierPlatformServiceFactory.Create(SourceConference.Supplier);
        videoPlatformService.TransferParticipantAsync(SourceConference.Id, telephoneParticipantId.ToString(),
            RoomType.WaitingRoom.ToString(), RoomType.HearingRoom.ToString(), ConferenceRole.Guest);
    }
}
