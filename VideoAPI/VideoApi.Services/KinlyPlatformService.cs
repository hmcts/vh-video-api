using System;
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
using VideoApi.Domain.Validations;
using VideoApi.Services.Exceptions;
using VideoApi.Services.Kinly;

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
                });

                var meetingRoom = new MeetingRoom(response.Uris.Admin, response.Uris.Judge, response.Uris.Participant,
                    response.Uris.Pexip_node);
                return meetingRoom;
            }
            catch (KinlyApiException e)
            {
                if (e.StatusCode == (int) HttpStatusCode.Conflict)
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
                if (e.StatusCode == (int) HttpStatusCode.NotFound)
                {
                    return null;
                }

                throw;
            }
        }

        public async Task<TestCallResult> GetTestCallScoreAsync(Guid participantId)
        {
            _logger.LogInformation(
                $"Retrieving test call score for participant {participantId} at {_servicesConfigOptions.KinlySelfTestApiUrl}");
            HttpResponseMessage responseMessage;
            using (var httpClient = new HttpClient())
            {
                var requestUri = $"{_servicesConfigOptions.KinlySelfTestApiUrl}/testcall/{participantId}";
                var request = new HttpRequestMessage()
                {
                    RequestUri = new Uri(requestUri),
                    Method = HttpMethod.Get,
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer",
                    _customJwtTokenProvider.GenerateToken(participantId.ToString(), 2));

                responseMessage = await httpClient.SendAsync(request);
            }
            
            if (!responseMessage.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await responseMessage.Content.ReadAsStringAsync();
            var testCall = ApiRequestHelper.DeserialiseSnakeCaseJsonToResponse<Testcall>(content);
            return new TestCallResult(testCall.Passed, (TestScore) testCall.Score);
        }
    }
}