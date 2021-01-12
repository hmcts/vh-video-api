using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VideoApi.Services.Contracts;
using VideoApi.Services.Kinly;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services
{
    public class ConsultationService : IConsultationService
    {
        private readonly IKinlyApiClient _kinlyApiClient;
        private readonly ILogger<ConsultationService> _logger;

        public ConsultationService(IKinlyApiClient kinlyApiClient, ILogger<ConsultationService> logger)
        {
            _kinlyApiClient = kinlyApiClient;
            _logger = logger;
        }
        
        public async Task<CreateConsultationRoomResponse> CreateConsultationRoomAsync(string virtualCourtRoomId, CreateConsultationRoomParams createConsultationRoomParams)
        {
            _logger.LogTrace("Creating a consultation for VirtualCourtRoomId: {virtualCourtRoomId} with prefix {createConsultationRoomParamsPrefix}",
                virtualCourtRoomId, createConsultationRoomParams.Room_label_prefix);

            CreateConsultationRoomResponse consultationRoomResponse;
            try
            {
                consultationRoomResponse = await _kinlyApiClient.CreateConsultationRoomAsync(virtualCourtRoomId, createConsultationRoomParams);
            }
            catch (KinlyApiException e)
            {
                _logger.LogError("Unable to create a consultation room for VirtualCourtRoomId: {virtualCourtRoomId}, with exception message: {e}", 
                    virtualCourtRoomId, e);
                throw;
            }

            return consultationRoomResponse;
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
            catch (KinlyApiException e)
            {
                _logger.LogError("Unable to start a transfer participant: {participantId} for ConferenceId: {{conferenceId}}, with exception message: {{e}}", 
                    participantId, conferenceId, e);
                throw;
            }
        }
    }
}
