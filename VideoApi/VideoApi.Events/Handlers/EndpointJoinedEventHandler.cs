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
    public class EndpointJoinedEventHandler : EventHandlerBase<EndpointJoinedEventHandler>
    {
        private readonly IVideoPlatformService _videoPlatformService;
        private readonly IFeatureToggles _featureToggles;

        public EndpointJoinedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler,
            ILogger<EndpointJoinedEventHandler> logger, IVideoPlatformService videoPlatformService,
            IFeatureToggles featureToggles) : base(
            queryHandler, commandHandler, logger)
        {
            _featureToggles = featureToggles;
            _videoPlatformService = videoPlatformService;
        }

        public override EventType EventType => EventType.EndpointJoined;

        protected override Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            const EndpointState endpointState = EndpointState.Connected;
            const RoomType room = RoomType.WaitingRoom;
            var command =
                new UpdateEndpointStatusAndRoomCommand(SourceConference.Id, SourceEndpoint.Id, endpointState, room,
                    null);

            if (_featureToggles.VodafoneIntegrationEnabled())
            {
                TransferToHearingRoomIfHearingIsAlreadyInSession();
            }

            return CommandHandler.Handle(command);
        }

        private void TransferToHearingRoomIfHearingIsAlreadyInSession()
        {
            if (SourceConference.State == ConferenceState.InSession)
            {
                _videoPlatformService.TransferParticipantAsync(SourceConference.Id, SourceEndpoint.Id.ToString(),
                    RoomType.WaitingRoom.ToString(), RoomType.HearingRoom.ToString());
            }
        }
    }
}
