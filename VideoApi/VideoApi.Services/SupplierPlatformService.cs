using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Clients;
using VideoApi.Services.Contracts;
using VideoApi.Services.Dtos;
using VideoApi.Services.Exceptions;
using VideoApi.Services.Mappers;
using Task = System.Threading.Tasks.Task;
using Supplier = VideoApi.Domain.Enums.Supplier;

namespace VideoApi.Services
{
    public class SupplierPlatformService : IVideoPlatformService
    {
        private readonly IFeatureToggles _featureToggles;
        private readonly ILogger<SupplierPlatformService> _logger;
        private readonly IPollyRetryService _pollyRetryService;
        private readonly Supplier _supplier;
        private readonly ISupplierApiClient _supplierApiClient;
        private readonly SupplierConfiguration _supplierConfigOptions;
        private readonly ISupplierSelfTestHttpClient _supplierSelfTestHttpClient;
        
        
        public SupplierPlatformService(
            ILogger<SupplierPlatformService> logger,
            ISupplierSelfTestHttpClient supplierSelfTestHttpClient,
            IPollyRetryService pollyRetryService,
            ISupplierApiClient supplierApiClient,
            SupplierConfiguration supplierConfiguration,
            Supplier supplier,
            IFeatureToggles featureToggles)
        {
            _logger = logger;
            _supplierSelfTestHttpClient = supplierSelfTestHttpClient;
            _pollyRetryService = pollyRetryService;
            _supplierApiClient = supplierApiClient;
            _supplierConfigOptions = supplierConfiguration;
            _supplier = supplier;
            _featureToggles = featureToggles;
        }
        
        
        public async Task<MeetingRoom> BookVirtualCourtroomAsync(Guid conferenceId,
            bool audioRecordingRequired,
            string ingestUrl,
            IEnumerable<EndpointDto> endpoints, 
            string telephoneId, ConferenceRoomType roomType, AudioPlaybackLanguage audioPlaybackLanguage)
        {
            _logger.LogInformation(
                "Booking a conference for {ConferenceId} with callback {CallbackUri} at {KinlyApiUrl}", conferenceId,
                _supplierConfigOptions.CallbackUri, _supplierConfigOptions.ApiUrl);
            try
            {
                var response = await _supplierApiClient.CreateHearingAsync(new CreateHearingParams
                {
                    Virtual_courtroom_id = conferenceId.ToString(),
                    RoomType = roomType.ToString(),
                    Callback_uri = _supplierConfigOptions.CallbackUri,
                    Recording_enabled = audioRecordingRequired,
                    Recording_url = ingestUrl,
                    Streaming_enabled = false,
                    Streaming_url = null,
                    Jvs_endpoint = endpoints.Select(EndpointMapper.MapToEndpoint).ToList(),
                    Telephone_Conference_id = telephoneId,
                    AudioPlaybackLanguage = audioPlaybackLanguage.ToString()
                });

                return _supplier == Supplier.Vodafone 
                    ? new MeetingRoom(response.Uris.Admin ?? response.Uris.Participant, response.Uris.Participant, response.Uris.Participant, response.Uris.Pexip_node, telephoneId)
                    : new MeetingRoom(response.Uris.Admin, response.Uris.Participant, response.Uris.Participant, response.Uris.Pexip_node, response.Telephone_conference_id);
            }
            catch (SupplierApiException e)
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
                var response = await _supplierApiClient.GetHearingAsync(conferenceId.ToString());
                var meetingRoom = new MeetingRoom(response.Uris.Admin, response.Uris.Participant,
                    response.Uris.Participant, response.Uris.Pexip_node, response.Telephone_conference_id);
                return meetingRoom;
            }
            catch (SupplierApiException e)
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
                        "Failed to retrieve test score for participant {ParticipantId} at {KinlySelfTestApiUrl}. Retrying attempt {RetryAttempt}",
                        participantId, _supplierConfigOptions.ApiUrl, retryAttempt),
                callResult => callResult == null,
                () => _supplierSelfTestHttpClient.GetTestCallScoreAsync(participantId)
            );

            return result;
        }
        
        public Task TransferParticipantAsync(Guid conferenceId, string participantId, string fromRoom, string toRoom, ConferenceRole? role = null)
        {
            _logger.LogInformation(
                "Transferring participant {ParticipantId} from {FromRoom} to {ToRoom} in conference: {ConferenceId}",
                participantId, fromRoom, toRoom, conferenceId);
            string roleString = null;
            if (role.HasValue)
            {
                roleString = role == ConferenceRole.Host ? "Host" : "Guest";
            }
            var request = new TransferParticipantParams
            {
                From = fromRoom,
                To = toRoom,
                Part_id = participantId
            };
            
            if (_featureToggles.SendTransferRolesEnabled())
                request.Role = roleString;
            
            return _supplierApiClient.TransferParticipantAsync(conferenceId.ToString(), request);
        }
        
        public Task DeleteVirtualCourtRoomAsync(Guid conferenceId)
        {
            return _supplierApiClient.DeleteHearingAsync(conferenceId.ToString());
        }
        
        public Task UpdateVirtualCourtRoomAsync(Guid conferenceId, bool audioRecordingRequired,
            IEnumerable<EndpointDto> endpoints, ConferenceRoomType roomType,
            AudioPlaybackLanguage audioPlaybackLanguage)
        {
            return _supplierApiClient.UpdateHearingAsync(conferenceId.ToString(),
                new UpdateHearingParams
                {
                    Recording_enabled = audioRecordingRequired,
                    Jvs_endpoint = endpoints.Select(EndpointMapper.MapToEndpoint).ToList(),
                    RoomType = roomType.ToString(),
                    AudioPlaybackLanguage = audioPlaybackLanguage.ToString()
                });
        }
        
        public Task StartHearingAsync(Guid conferenceId, string triggeredByHostId,
            IEnumerable<string> participantsToForceTransfer = null, IEnumerable<string> hosts = null,
            Layout layout = Layout.AUTOMATIC, bool muteGuests = false, IEnumerable<string> hostsForScreening =null)
        {
            if (_supplier != Supplier.Vodafone)
                triggeredByHostId = null;
            return _supplierApiClient.StartAsync(conferenceId.ToString(),
                new StartHearingRequest
                {
                    Hearing_layout = layout, Mute_guests = muteGuests,
                    Force_transfer_participant_ids = participantsToForceTransfer?.ToList(),
                    Hosts = hosts?.ToList(),
                    Triggered_by_host_id = triggeredByHostId,
                    HostsForScreening = hostsForScreening?.ToList()
                });
        }
        
        public Task PauseHearingAsync(Guid conferenceId)
        {
            return _supplierApiClient.PauseHearingAsync(conferenceId.ToString());
        }
        
        public Task EndHearingAsync(Guid conferenceId)
        {
            return _supplierApiClient.EndHearingAsync(conferenceId.ToString());
        }
        
        public Task SuspendHearingAsync(Guid conferenceId)
        {
            return _supplierApiClient.TechnicalAssistanceAsync(conferenceId.ToString());
        }
        
        public Task<HealthCheckResponse> GetPlatformHealthAsync()
        {
            return _supplierApiClient.HealthCheckAsync();
        }
        
        public ISupplierApiClient GetHttpClient() => _supplierApiClient;
        
        public SupplierConfiguration GetSupplierConfiguration() => _supplierConfigOptions;
        
        public Task UpdateParticipantName(Guid conferenceId, Guid participantId, string name)
        {
            var request = new UpdateParticipantNameParams()
            {
                Participant_Id = participantId.ToString(),
                Participant_Name = name
            };
            return _supplierApiClient.UpdateParticipanNameAsync(conferenceId.ToString(), request);
        }
    }
}
