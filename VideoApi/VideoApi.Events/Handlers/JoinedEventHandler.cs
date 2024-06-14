using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain.Enums;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using VideoApi.Services;
using VideoApi.Services.Contracts;

namespace VideoApi.Events.Handlers
{
    public class JoinedEventHandler : EventHandlerBase<JoinedEventHandler>
    {
        private readonly IVideoPlatformService _videoPlatformService;
        private readonly IFeatureToggles _featureToggles;
        
        public JoinedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler, ILogger<JoinedEventHandler> logger, IVideoPlatformService videoPlatformService, IFeatureToggles featureToggles) : base(
            queryHandler, commandHandler, logger)
        {
            _videoPlatformService = videoPlatformService;
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
                _videoPlatformService.TransferParticipantAsync(SourceConference.Id, SourceParticipant.Id.ToString(),
                    RoomType.WaitingRoom.ToString(), RoomType.HearingRoom.ToString());
            }
            
            if(SourceConference.State == ConferenceState.InSession && SourceEndpoint != null)
            {
                _videoPlatformService.TransferParticipantAsync(SourceConference.Id, SourceEndpoint.Id.ToString(),
                    RoomType.WaitingRoom.ToString(), RoomType.HearingRoom.ToString());
            }
        }
    }
}
