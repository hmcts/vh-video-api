using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using VideoApi.DAL.Commands;
using VideoApi.DAL.Commands.Core;
using VideoApi.DAL.Queries.Core;
using VideoApi.Events.Handlers.Core;
using VideoApi.Events.Models;
using VideoApi.Services;
using ConferenceState = VideoApi.Domain.Enums.ConferenceState;
using EndpointState = VideoApi.Domain.Enums.EndpointState;
using EventType = VideoApi.Domain.Enums.EventType;
using RoomType = VideoApi.Domain.Enums.RoomType;
using Supplier = VideoApi.Domain.Enums.Supplier;

namespace VideoApi.Events.Handlers
{
    public class EndpointJoinedEventHandler : EventHandlerBase<EndpointJoinedEventHandler>
    {
        private readonly ISupplierPlatformServiceFactory _supplierPlatformServiceFactory;

        public EndpointJoinedEventHandler(IQueryHandler queryHandler, ICommandHandler commandHandler,
            ILogger<EndpointJoinedEventHandler> logger, ISupplierPlatformServiceFactory supplierPlatformServiceFactory) : base(
            queryHandler, commandHandler, logger)
        {
            _supplierPlatformServiceFactory = supplierPlatformServiceFactory;
        }

        public override EventType EventType => EventType.EndpointJoined;

        protected override Task PublishStatusAsync(CallbackEvent callbackEvent)
        {
            const EndpointState endpointState = EndpointState.Connected;
            const RoomType room = RoomType.WaitingRoom;
            var command =
                new UpdateEndpointStatusAndRoomCommand(SourceConference.Id, SourceEndpoint.Id, endpointState, room,
                    null);
            
            _logger.LogInformation("Endpoint joined callback - {ConferenceId}/{EndpointId}",
                SourceConference.Id, SourceEndpoint.Id);
            
            if (SourceConference.Supplier == Supplier.Vodafone)
            {
                _logger.LogInformation("Vodafone integration enabled, transferring endpoint {EndpointId} to hearing room if in session",
                    SourceEndpoint.Id);
                TransferToHearingRoomIfHearingIsAlreadyInSession();
            }

            return CommandHandler.Handle(command);
        }

        private void TransferToHearingRoomIfHearingIsAlreadyInSession()
        {
            _logger.LogInformation("Conference {ConferenceId} state is {ConferenceState}", SourceConference.Id,
                SourceConference.State.ToString());
            if (SourceConference.State == ConferenceState.InSession)
            {
                _logger.LogInformation("Conference {ConferenceId} already in session, transferring endpoint {EndpointId} to hearing room",
                    SourceConference.Id, SourceEndpoint.Id);
                var videoPlatformService = _supplierPlatformServiceFactory.Create(SourceConference.Supplier);
                videoPlatformService.TransferParticipantAsync(SourceConference.Id, SourceEndpoint.Id.ToString(),
                    RoomType.WaitingRoom.ToString(), RoomType.HearingRoom.ToString());
            }
        }
    }
}
