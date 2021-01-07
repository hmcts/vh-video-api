using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VideoApi.Domain.Enums;
using VideoApi.Services.Contracts;
using VideoApi.Services.Kinly;

namespace VideoApi.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly IKinlyApiClient _kinlyApiClient;
        private readonly ILogger<KinlyPlatformService> _logger;

        public ConsultationService(IKinlyApiClient kinlyApiClient, ILogger<KinlyPlatformService> logger)
        {
            _kinlyApiClient = kinlyApiClient;
            _logger = logger;
        }
        
        public async Task StartConsultationAsync(Guid conferenceId, Guid requestedBy, RoomType roomType)
        {
            _logger.LogTrace("Starting a consultation for ConferenceId: {conferenceId} requested by {requestedBy} with {roomType}",
                conferenceId, requestedBy, roomType);

            try
            {
                await _kinlyApiClient.CreateConsultationRoomAsync(roomType.ToString(), new CreateConsultationRoomParams {Room_label_prefix = "Enter prefix here"});
                
                //TODO: Change RoomStatus to "Created"
            }
            catch (Exception e)
            {
                _logger.LogError("Unable to start a consultation for ConferenceId: {conferenceId}, with exception message: {e}", 
                    conferenceId, e);
            }
        }

        public Task TransferParticipantAsync(Guid conferenceId, Guid participantId, RoomType fromRoom, RoomType toRoom)
        {
            _logger.LogInformation(
                "Transferring participant {participantId} from {fromRoom} to {toRoom} in conference: {conferenceId}",
                participantId, fromRoom, toRoom, conferenceId);

            var request = new TransferParticipantParams
            {
                From = fromRoom.ToString(),
                To = toRoom.ToString(),
                Part_id = participantId.ToString()
            };

            return _kinlyApiClient.TransferParticipantAsync(conferenceId.ToString(), request);
        }
    }
}
