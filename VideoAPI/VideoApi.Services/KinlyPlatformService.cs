using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VideoApi.Common.Configuration;
using VideoApi.Common.Helpers;
using VideoApi.Common.Security.CustomToken;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Exceptions;
using VideoApi.Services.Kinly;
using Task = System.Threading.Tasks.Task;
using Polly;

namespace VideoApi.Services
{
    public class KinlyPlatformService : IVideoPlatformService
    {
        private readonly IKinlyApiClient _kinlyApiClient;
        private readonly ICustomJwtTokenProvider _customJwtTokenProvider;
        private readonly ILogger<KinlyPlatformService> _logger;
        private readonly ServicesConfiguration _servicesConfigOptions;

        public KinlyPlatformService(IKinlyApiClient kinlyApiClient,
            IOptions<ServicesConfiguration> servicesConfigOptions,
            ICustomJwtTokenProvider customJwtTokenProvider, ILogger<KinlyPlatformService> logger)
        {
            _kinlyApiClient = kinlyApiClient;
            _customJwtTokenProvider = customJwtTokenProvider;
            _logger = logger;
            _servicesConfigOptions = servicesConfigOptions.Value;
        }


        public async Task<MeetingRoom> BookVirtualCourtroomAsync(Guid conferenceId)
        {
            _logger.LogInformation(
                $"Booking a conference for {conferenceId} with callback {_servicesConfigOptions.CallbackUri} at {_servicesConfigOptions.KinlyApiUrl}");
            try
            {
                var response = await _kinlyApiClient.CreateHearingAsync(new CreateHearingParams
                {
                    Virtual_courtroom_id = conferenceId.ToString(),
                    Callback_uri = _servicesConfigOptions.CallbackUri
                }).ConfigureAwait(false);

                var meetingRoom = new MeetingRoom(response.Uris.Admin, response.Uris.Judge, response.Uris.Participant,
                    response.Uris.Pexip_node);
                return meetingRoom;
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
                var response = await _kinlyApiClient.GetHearingAsync(conferenceId.ToString()).ConfigureAwait(false);
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
            var maxRetryAttempts = 2;
            var pauseBetweenFailures = TimeSpan.FromSeconds(5);

            var policy = Policy
                .Handle<Exception>()
                .OrResult<TestCallResult>(r => r == null)
                .WaitAndRetryAsync(maxRetryAttempts, x => pauseBetweenFailures, (ex, ts, retryAttempt, context) =>
                {
                    _logger.LogError($"Failed to retrieve test score for participant {participantId} at {_servicesConfigOptions.KinlySelfTestApiUrl}. Retrying attempt { retryAttempt }");
                });

            return await policy
                .ExecuteAsync(async () => await GetSelfTestCallScore(participantId))
                .ConfigureAwait(false);
        }

        public async Task<TestCallResult> GetSelfTestCallScore(Guid participantId)
        {
            _logger.LogInformation(
                $"Retrieving test call score for participant {participantId} at {_servicesConfigOptions.KinlySelfTestApiUrl}");
            HttpResponseMessage responseMessage;
            using (var httpClient = new HttpClient())
            {
                var requestUri = $"{_servicesConfigOptions.KinlySelfTestApiUrl}/testcall/{participantId}";
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(requestUri),
                    Method = HttpMethod.Get,
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",
                    _customJwtTokenProvider.GenerateToken(participantId.ToString(), 2));

                responseMessage = await httpClient.SendAsync(request).ConfigureAwait(false);
            }

            if (responseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogError($" { responseMessage.StatusCode } : Failed to retrieve self test score for participant {participantId} ");
                return null;
            }

            var content = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            var testCall = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<Testcall>(content);
            _logger.LogError($" { responseMessage.StatusCode } : Successfully retrieved self test score for participant {participantId} ");
            return new TestCallResult(testCall.Passed, (TestScore)testCall.Score);
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

            await _kinlyApiClient.TransferParticipantAsync(conferenceId.ToString(), request).ConfigureAwait(false);
        }

        public async Task StartPrivateConsultationAsync(Conference conference, Participant requestedBy, Participant requestedFor)
        {
            var targetRoom = conference.GetAvailableConsultationRoom();

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
            await _kinlyApiClient.DeleteHearingAsync(conferenceId.ToString()).ConfigureAwait(false);
        }
    }
}