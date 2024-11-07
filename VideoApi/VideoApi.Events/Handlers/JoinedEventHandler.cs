using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using VideoApi.Services;
using ConferenceState = VideoApi.Domain.Enums.ConferenceState;
using EventType = VideoApi.Domain.Enums.EventType;
using ParticipantState = VideoApi.Domain.Enums.ParticipantState;
using RoomType = VideoApi.Domain.Enums.RoomType;
using Supplier = VideoApi.Domain.Enums.Supplier;

namespace VideoApi.Events.Handlers
{
    public class JoinedEventHandler : EventHandlerBase<JoinedEventHandler>
    {
        private readonly ISupplierPlatformServiceFactory _supplierPlatformServiceFactory;

        public JoinedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<JoinedEventHandler> logger, ISupplierPlatformServiceFactory supplierPlatformServiceFactory) : base(
            queryHandler, commandHandler, logger)
        {
            _supplierPlatformServiceFactory = supplierPlatformServiceFactory;
        }

        public override EventType EventType => EventType.Joined;

        protected override Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var participantState =  ParticipantState.Available;
            var room = RoomType.WaitingRoom;
            var command = new UpdateParticipantStatusAndRoomCommand(SourceConference.Id, SourceParticipant.Id,
                participantState, room, null);
            
            Logger.LogInformation("Joined callback - {ConferenceId}/{ParticipantId}",
                SourceConference.Id, SourceParticipant.Id);
            
            if (SourceConference.Supplier == Supplier.Vodafone)
            {
                TransferToHearingRoomIfHearingIsAlreadyInSession();
            }
            
            return CommandHandler.Handle(command);
        }
        
        private void TransferToHearingRoomIfHearingIsAlreadyInSession()
        {
            if (SourceConference.State == ConferenceState.InSession && SourceParticipant.CanAutoTransferToHearingRoom())
            {
                var videoPlatformService = _supplierPlatformServiceFactory.Create(SourceConference.Supplier);
                videoPlatformService.TransferParticipantAsync(SourceConference.Id, SourceParticipant.Id.ToString(),
                    RoomType.WaitingRoom.ToString(), RoomType.HearingRoom.ToString());
            }
        }
    }
}
