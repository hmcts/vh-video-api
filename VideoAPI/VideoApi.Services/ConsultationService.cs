using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VideoApi.Domain.Enums;
using VideoApi.Services.Contracts;
using VideoApi.Services.Kinly;
using Task = System.Threading.Tasks.Task;

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
        
        public Task<CreateConsultationRoomResponse> CreateConsultationRoomAsync(string virtualCourtRoomId, CreateConsultationRoomParams createConsultationRoomParams)
        {
            _logger.LogTrace("Creating a consultation for VirtualCourtRoomId: {virtualCourtRoomId} with prefix {createConsultationRoomParamsPrefix}",
                virtualCourtRoomId, createConsultationRoomParams.Room_label_prefix);

            try
            {
                return _kinlyApiClient.CreateConsultationRoomAsync(virtualCourtRoomId, createConsultationRoomParams);
            }
            catch (KinlyApiException e)
            {
                _logger.LogError("Unable to create a consultation room for VirtualCourtRoomId: {virtualCourtRoomId}, with exception message: {e}", 
                    virtualCourtRoomId, e);
                return Task.FromResult<CreateConsultationRoomResponse>(null);
            }
        }

        public async Task TransferParticipantAsync(Guid conferenceId, Guid participantId, string fromRoom, string toRoom)
        {
            _logger.LogTrace(
                "Transferring participant {participantId} from {fromRoom} to {toRoom} in conference: {conferenceId}",
                participantId, fromRoom, toRoom, conferenceId);

            var request = new TransferParticipantParams
            {
                From = fromRoom,
                To = toRoom,
                Part_id = participantId.ToString()
            };
            try
            {
                await _kinlyApiClient.TransferParticipantAsync(conferenceId.ToString(), request);
            }
            catch (Exception e)
            {
                _logger.LogError("Unable to start a transfer participant: {participantId} for ConferenceId: {{conferenceId}}, with exception message: {{e}}", 
                    participantId, conferenceId, e);
            }
        }
    }
}
