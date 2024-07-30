using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VideoApi.Contract.Enums;
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

namespace VideoApi.Events.Handlers
{
    public class JoinedEventHandler : EventHandlerBase<JoinedEventHandler>
    {
        private readonly ISupplierPlatformServiceFactory _supplierPlatformServiceFactory;
        private readonly IFeatureToggles _featureToggles;
        
        public JoinedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<JoinedEventHandler> logger, ISupplierPlatformServiceFactory supplierPlatformServiceFactory, IFeatureToggles featureToggles) : base(
            queryHandler, commandHandler, logger)
        {
            _supplierPlatformServiceFactory = supplierPlatformServiceFactory;
            _featureToggles = featureToggles;
        }

        public override EventType EventType => EventType.Joined;

        protected override Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            var participantState =  ParticipantState.Available;
            var room = RoomType.WaitingRoom;
            var command = new UpdateParticipantStatusAndRoomCommand(SourceConference.Id, SourceParticipant.Id,
                participantState, room, null);
            
            _logger.LogInformation("Joined callback - {ConferenceId}/{ParticipantId}",
                SourceConference.Id, SourceParticipant.Id);
            
            if (_featureToggles.VodafoneIntegrationEnabled())
            {
                TransferToHearingRoomIfHearingIsAlreadyInSession();
            }
            
            return CommandHandler.Handle(command);
        }
        
        private void TransferToHearingRoomIfHearingIsAlreadyInSession()
        {
            if (SourceConference.State == ConferenceState.InSession && SourceParticipant.CanAutoTransferToHearingRoom())
            {
                var videoPlatformService = _supplierPlatformServiceFactory.Create(Supplier.Kinly);
                videoPlatformService.TransferParticipantAsync(SourceConference.Id, SourceParticipant.Id.ToString(),
                    RoomType.WaitingRoom.ToString(), RoomType.HearingRoom.ToString());
            }
        }
    }
}
