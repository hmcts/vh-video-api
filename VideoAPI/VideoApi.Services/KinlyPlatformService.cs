using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VideoApi.Common.Configuration;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Exceptions;
using VideoApi.Services.Kinly;
using Task = System.Threading.Tasks.Task;
using VideoApi.Services.Contracts;

namespace VideoApi.Services
{
    public class KinlyPlatformService : IVideoPlatformService
    {
        private readonly IKinlyApiClient _kinlyApiClient;
        private readonly ILogger<KinlyPlatformService> _logger;
        private readonly ServicesConfiguration _servicesConfigOptions;
        private readonly IRoomReservationService _roomReservationService;
        private readonly IKinlySelfTestHttpClient _kinlySelfTestHttpClient;
        private readonly IPollyRetryService _pollyRetryService;

        public KinlyPlatformService(IKinlyApiClient kinlyApiClient, IOptions<ServicesConfiguration> servicesConfigOptions,
            ILogger<KinlyPlatformService> logger, IRoomReservationService roomReservationService, IKinlySelfTestHttpClient kinlySelfTestHttpClient,
            IPollyRetryService pollyRetryService)
        {
            _kinlyApiClient = kinlyApiClient;
            _logger = logger;
            _servicesConfigOptions = servicesConfigOptions.Value;
            _roomReservationService = roomReservationService;
            _kinlySelfTestHttpClient = kinlySelfTestHttpClient;
            _pollyRetryService = pollyRetryService;
        }


        public async Task<MeetingRoom> BookVirtualCourtroomAsync(Guid conferenceId, bool audioRecordingRequired, string ingestUrl)
        {
            _logger.LogInformation($"Booking a conference for {conferenceId} with callback {_servicesConfigOptions.CallbackUri} at {_servicesConfigOptions.KinlyApiUrl}");
            
            try
            {
                var response = await _kinlyApiClient.CreateHearingAsync(new CreateHearingParams
                {
                    Virtual_courtroom_id = conferenceId.ToString(),
                    Callback_uri = _servicesConfigOptions.CallbackUri,
                    Recording_enabled = audioRecordingRequired,
                    Recording_url = ingestUrl,
                    Streaming_enabled = false,
                    Streaming_url = null
                });

                return new MeetingRoom
                (
                    response.Uris.Admin, response.Uris.Judge, response.Uris.Participant, response.Uris.Pexip_node
                );
            }
            catch (KinlyApiException e)
            {
                if (e.StatusCode == (int)HttpStatusCode.Conflict)
                {
                    throw new DoubleBookingException(conferenceId, e.Message);
                }

                throw;
            }
        }

        public async Task<MeetingRoom> GetVirtualCourtRoomAsync(Guid conferenceId)
        {
            try
            {
                var response = await _kinlyApiClient.GetHearingAsync(conferenceId.ToString());
                var meetingRoom = new MeetingRoom(response.Uris.Admin, response.Uris.Judge, response.Uris.Participant,
                    response.Uris.Pexip_node);
                return meetingRoom;
            }
            catch (KinlyApiException e)
            {
                if (e.StatusCode == (int)HttpStatusCode.NotFound)
                {
                    return null;
                }

                throw;
            }
        }

        public async Task<TestCallResult> GetTestCallScoreAsync(Guid participantId)
        {
            const int maxRetryAttempts = 2;
            var pauseBetweenFailures = TimeSpan.FromSeconds(5);

            var result = await _pollyRetryService.WaitAndRetryAsync<Exception, TestCallResult>
            (
                maxRetryAttempts, 
                _ => pauseBetweenFailures,
                retryAttempt => _logger.LogWarning($"Failed to retrieve test score for participant {participantId} at {_servicesConfigOptions.KinlySelfTestApiUrl}. Retrying attempt {retryAttempt}"),
                callResult => callResult == null, 
                () => _kinlySelfTestHttpClient.GetTestCallScoreAsync(participantId)
            );

            return result;
        }

        public async Task TransferParticipantAsync(Guid conferenceId, Guid participantId, RoomType fromRoom,
            RoomType toRoom)
        {
            _logger.LogInformation(
                $"Transferring participant {participantId} from {fromRoom} to {toRoom} in conference: {conferenceId}");

            var request = new TransferParticipantParams
            {
                From = fromRoom.ToString(),
                To = toRoom.ToString(),
                Part_id = participantId.ToString()
            };

            await _kinlyApiClient.TransferParticipantAsync(conferenceId.ToString(), request);
        }

        public async Task StartPrivateConsultationAsync(Conference conference, Participant requestedBy,
            Participant requestedFor)
        {
            var targetRoom = _roomReservationService.GetNextAvailableConsultationRoom(conference);

            _logger.LogInformation(
                $"Conference: {conference.Id} - Attempting to transfer participants {requestedBy.Id} {requestedFor.Id} into room {targetRoom}");

            await TransferParticipantAsync(conference.Id, requestedBy.Id,
                requestedBy.CurrentRoom.Value, targetRoom);

            await TransferParticipantAsync(conference.Id, requestedFor.Id,
                requestedFor.CurrentRoom.Value, targetRoom);
        }

        public async Task StopPrivateConsultationAsync(Conference conference, RoomType consultationRoom)
        {
            var participants = conference.GetParticipants()
                .Where(x => x.CurrentRoom.HasValue && x.CurrentRoom == consultationRoom);

            foreach (var participant in participants)
            {
                await TransferParticipantAsync(conference.Id, participant.Id, consultationRoom,
                    RoomType.WaitingRoom);
            }
        }

        public async Task DeleteVirtualCourtRoomAsync(Guid conferenceId)
        {
            await _kinlyApiClient.DeleteHearingAsync(conferenceId.ToString());
        }

        public async Task UpdateVirtualCourtRoomAsync(Guid conferenceId, bool audioRecordingRequired)
        {
            await _kinlyApiClient.UpdateHearingAsync(conferenceId.ToString(), new UpdateHearingParams {Recording_enabled = audioRecordingRequired});
        }

        public async Task StartHearingAsync(Guid conferenceId)
        {
            await _kinlyApiClient.StartHearingAsync(conferenceId.ToString());
        }

        public async Task PauseHearingAsync(Guid conferenceId)
        {
            await _kinlyApiClient.PauseHearingAsync(conferenceId.ToString());
        }

        public async Task EndHearingAsync(Guid conferenceId)
        {
            await _kinlyApiClient.EndHearingAsync(conferenceId.ToString());
        }
    }
}
