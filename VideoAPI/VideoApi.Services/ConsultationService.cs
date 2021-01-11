using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VideoApi.Domain;
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


        public Task TransferParticipantAsync(Guid conferenceId, Guid participantId, VirtualCourtRoomType fromRoom,
            VirtualCourtRoomType toRoom)
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

        public async Task EndJudgeJohConsultationAsync(Guid conferenceId, Room room)
        {
            var participants = room.GetRoomParticipants();
            foreach (var participant in participants)
            {
                await TransferParticipantAsync(conferenceId, participant.ParticipantId,
                    VirtualCourtRoomType.JudgeJOH, VirtualCourtRoomType.WaitingRoom);
            }
        }
    }
}
