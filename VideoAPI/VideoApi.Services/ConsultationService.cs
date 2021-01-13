using Microsoft.Extensions.Logging;
using System;
using VideoApi.Contract.Requests;
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


        private async Task TransferParticipantAsync(Guid conferenceId, Guid participantId, string fromRoom,
            string toRoom)
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

            await _kinlyApiClient.TransferParticipantAsync(conferenceId.ToString(), request);
        }

        public async Task LeaveConsultationAsync(LeaveConsultationRequest request, string fromRoom, string toRoom)
        {
            await TransferParticipantAsync(request.ConferenceId, request.ParticipantId, fromRoom, toRoom);
        }
    }
}
