using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VideoApi.Common.Security.Kinly;
using VideoApi.Domain;
using VideoApi.Services.Exceptions;
using VideoApi.Services.Clients;
using Task = System.Threading.Tasks.Task;
using VideoApi.Services.Contracts;
using VideoApi.Services.Dtos;
using VideoApi.Services.Mappers;

namespace VideoApi.Services
{
    public class KinlyPlatformService : IVideoPlatformService
    {
        private readonly IKinlyApiClient _kinlyApiClient;
        private readonly ILogger<KinlyPlatformService> _logger;
        private readonly KinlyConfiguration _kinlyConfigOptions;
        private readonly IKinlySelfTestHttpClient _kinlySelfTestHttpClient;
        private readonly IPollyRetryService _pollyRetryService;

        public KinlyPlatformService(IKinlyApiClient kinlyApiClient,
            IOptions<KinlyConfiguration> kinlyConfigOptions,
            ILogger<KinlyPlatformService> logger,
            IKinlySelfTestHttpClient kinlySelfTestHttpClient,
            IPollyRetryService pollyRetryService)
        {
            _kinlyApiClient = kinlyApiClient;
            _logger = logger;
            _kinlyConfigOptions = kinlyConfigOptions.Value;
            _kinlySelfTestHttpClient = kinlySelfTestHttpClient;
            _pollyRetryService = pollyRetryService;
        }


        public async Task<MeetingRoom> BookVirtualCourtroomAsync(Guid conferenceId,
            bool audioRecordingRequired,
            string ingestUrl,
            IEnumerable<EndpointDto> endpoints)
        {
            _logger.LogInformation(
                "Booking a conference for {ConferenceId} with callback {CallbackUri} at {KinlyApiUrl}", conferenceId,
                _kinlyConfigOptions.CallbackUri, _kinlyConfigOptions.KinlyApiUrl);

            try
            {
                var response = await _kinlyApiClient.CreateHearingAsync(new CreateHearingParams
                {
                    Virtual_courtroom_id = conferenceId.ToString(),
                    Callback_uri = _kinlyConfigOptions.CallbackUri,
                    Recording_enabled = audioRecordingRequired,
                    Recording_url = ingestUrl,
                    Streaming_enabled = false,
                    Streaming_url = null,
                    Jvs_endpoint = endpoints.Select(EndpointMapper.MapToEndpoint).ToList()
                });

                return new MeetingRoom
                (response.Uris.Admin, response.Uris.Participant, response.Uris.Participant,
                    response.Uris.Pexip_node, response.Telephone_conference_id);
            }
            catch (KinlyApiException e)
            {
                if (e.StatusCode == (int) HttpStatusCode.Conflict)
                {
                    throw new DoubleBookingException(conferenceId);
                }

                throw;
            }
        }

        public async Task<MeetingRoom> GetVirtualCourtRoomAsync(Guid conferenceId)
        {
            try
            {
                var response = await _kinlyApiClient.GetHearingAsync(conferenceId.ToString());
                var meetingRoom = new MeetingRoom(response.Uris.Admin, response.Uris.Participant,
                    response.Uris.Participant, response.Uris.Pexip_node, response.Telephone_conference_id);
                return meetingRoom;
            }
            catch (KinlyApiException e)
            {
                if (e.StatusCode == (int) HttpStatusCode.NotFound)
                {
                    return null;
                }

                throw;
            }
        }

        public async Task<TestCallResult> GetTestCallScoreAsync(Guid participantId, int retryAttempts = 2)
        {
            var maxRetryAttempts = retryAttempts;
            var pauseBetweenFailures = TimeSpan.FromSeconds(5);

            var result = await _pollyRetryService.WaitAndRetryAsync<Exception, TestCallResult>
            (
                maxRetryAttempts,
                _ => pauseBetweenFailures,
                retryAttempt =>
                    _logger.LogWarning(
                        "Failed to retrieve test score for participant {ParticipantId} at {KinlySelfTestApiUrl}. Retrying attempt {retryAttempt}",
                        participantId, _kinlyConfigOptions.KinlySelfTestApiUrl, retryAttempt),
                callResult => callResult == null,
                () => _kinlySelfTestHttpClient.GetTestCallScoreAsync(participantId)
            );

            return result;
        }

        public Task TransferParticipantAsync(Guid conferenceId, string participantId, string fromRoom,
            string toRoom)
        {
            _logger.LogInformation(
                "Transferring participant {ParticipantId} from {FromRoom} to {ToRoom} in conference: {ConferenceId}",
                participantId, fromRoom, toRoom, conferenceId);

            var request = new TransferParticipantParams
            {
                From = fromRoom,
                To = toRoom,
                Part_id = participantId.ToString()
            };

            return _kinlyApiClient.TransferParticipantAsync(conferenceId.ToString(), request);
        }

        public Task DeleteVirtualCourtRoomAsync(Guid conferenceId)
        {
            return _kinlyApiClient.DeleteHearingAsync(conferenceId.ToString());
        }

        public Task UpdateVirtualCourtRoomAsync(Guid conferenceId, bool audioRecordingRequired,
            IEnumerable<EndpointDto> endpoints)
        {
            return _kinlyApiClient.UpdateHearingAsync(conferenceId.ToString(),
                new UpdateHearingParams
                {
                    Recording_enabled = audioRecordingRequired,
                    Jvs_endpoint = endpoints.Select(EndpointMapper.MapToEndpoint).ToList()
                });
        }

        public Task StartHearingAsync(Guid conferenceId, IEnumerable<string> participantsToForceTransfer = null, Layout layout = Layout.AUTOMATIC, bool muteGuests = false)
        {
            return _kinlyApiClient.StartAsync(conferenceId.ToString(),
                new StartHearingRequest { Hearing_layout = layout, Mute_guests = muteGuests, Force_transfer_participant_ids = participantsToForceTransfer?.ToList()});
        }

        public Task PauseHearingAsync(Guid conferenceId)
        {
            return _kinlyApiClient.PauseHearingAsync(conferenceId.ToString());
        }

        public Task EndHearingAsync(Guid conferenceId)
        {
            return _kinlyApiClient.EndHearingAsync(conferenceId.ToString());
        }

        public Task SuspendHearingAsync(Guid conferenceId)
        {
            return _kinlyApiClient.TechnicalAssistanceAsync(conferenceId.ToString());
        }

        public Task<HealthCheckResponse> GetPlatformHealthAsync()
        {
            return _kinlyApiClient.HealthCheckAsync();
        }
    }
}
