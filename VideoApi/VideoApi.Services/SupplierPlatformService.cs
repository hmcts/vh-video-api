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
using VideoApi.Services.Clients.Models;
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
        private readonly ILogger<SupplierPlatformService> _logger;
        private readonly IPollyRetryService _pollyRetryService;
        private readonly Supplier _supplier;
        private readonly ISupplierApiClient _supplierApiClient;
        private readonly SupplierConfiguration _supplierConfigOptions;
        
        
        public SupplierPlatformService(
            ILogger<SupplierPlatformService> logger,
            IPollyRetryService pollyRetryService,
            ISupplierApiClient supplierApiClient,
            SupplierConfiguration supplierConfiguration,
            Supplier supplier)
        {
            _logger = logger;
            _pollyRetryService = pollyRetryService;
            _supplierApiClient = supplierApiClient;
            _supplierConfigOptions = supplierConfiguration;
            _supplier = supplier;
        }
        
        
        public async Task<MeetingRoom> BookVirtualCourtroomAsync(Guid conferenceId,
            bool audioRecordingRequired,
            string ingestUrl,
            IEnumerable<EndpointDto> endpoints, 
            string telephoneId, ConferenceRoomType roomType, AudioPlaybackLanguage audioPlaybackLanguage)
        {
            _logger.LogInformation(
                "Booking a conference for {ConferenceId} with callback {CallbackUri} at {SupplierApiUrl}", conferenceId,
                _supplierConfigOptions.CallbackUri, _supplierConfigOptions.ApiUrl);
            try
            {
                var response = await _supplierApiClient.CreateHearingAsync(new BookHearingRequest
                {
                    VirtualCourtroomId = conferenceId,
                    RoomType = roomType.ToString(),
                    CallbackUri = _supplierConfigOptions.CallbackUri,
                    RecordingEnabled = audioRecordingRequired,
                    RecordingUrl = ingestUrl,
                    StreamingEnabled = false,
                    StreamingUrl = null,
                    JvsEndpoint = endpoints.Select(EndpointMapper.MapToEndpoint).ToList(),
                    TelephoneConferenceId = telephoneId,
                    AudioPlaybackLanguage = audioPlaybackLanguage.ToString()
                });

                return new MeetingRoom(response.Uris.Participant, response.Uris.Participant, response.Uris.Participant,
                    response.Uris.PexipNode, telephoneId);
            }
            catch (SupplierApiException  e)
            {
                if (e.StatusCode == HttpStatusCode.Conflict)
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
                var response = await _supplierApiClient.GetHearingAsync(conferenceId);
                var meetingRoom = new MeetingRoom(response.Uris.Participant, response.Uris.Participant,
                    response.Uris.Participant, response.Uris.PexipNode, null);
                return meetingRoom;
            }
            catch (SupplierApiException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
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

            var testCall = await _pollyRetryService.WaitAndRetryAsync<Exception, SelfTestParticipantResponse>
            (
                maxRetryAttempts,
                _ => pauseBetweenFailures,
                retryAttempt =>
                    _logger.LogWarning(
                        "Failed to retrieve test score for participant {ParticipantId} at {SupplierSelfTestApiUrl}. Retrying attempt {RetryAttempt}",
                        participantId, _supplierConfigOptions.ApiUrl, retryAttempt),
                callResult => callResult == null,
                () => _supplierApiClient.RetrieveParticipantSelfTestScore(participantId)
            );

            return new TestCallResult(testCall.Passed, (TestScore)testCall.Score);
        }
        
        public Task TransferParticipantAsync(Guid conferenceId, string participantId, string fromRoom, string toRoom, ConferenceRole? role = null)
        {
            _logger.LogInformation(
                "Transferring participant {ParticipantId} from {FromRoom} to {ToRoom} as {ConferenceRole} in conference: {ConferenceId}",
                participantId, fromRoom, toRoom, role, conferenceId);
            
            var request = new TransferRequest
            {
                From = fromRoom,
                To = toRoom,
                PartId = participantId,
                Role = role?.ToString()
            };
            
            return _supplierApiClient.TransferParticipantAsync(conferenceId, request);
        }
        
        public Task DeleteVirtualCourtRoomAsync(Guid conferenceId)
        {
            return _supplierApiClient.DeleteHearingAsync(conferenceId);
        }
        
        public Task UpdateVirtualCourtRoomAsync(Guid conferenceId, bool audioRecordingRequired,
            IEnumerable<EndpointDto> endpoints, ConferenceRoomType roomType,
            AudioPlaybackLanguage audioPlaybackLanguage)
        {
            return _supplierApiClient.UpdateHearingAsync(conferenceId,
                new UpdateHearingRequest
                {
                    RecordingEnabled = audioRecordingRequired,
                    JvsEndpoint = endpoints.Select(EndpointMapper.MapToEndpoint).ToList(),
                    RoomType = roomType.ToString(),
                    AudioPlaybackLanguage = audioPlaybackLanguage.ToString()
                });
        }

        public Task StartHearingAsync(Guid conferenceId, string triggeredByHostId, IEnumerable<string> participantsToForceTransfer = null,
            IEnumerable<string> hosts = null, Layout layout = Layout.Automatic, bool muteGuests = false,
            IEnumerable<string> hostsForScreening = null)
        {
            if (_supplier != Supplier.Vodafone)
                triggeredByHostId = null;
            return _supplierApiClient.StartAsync(conferenceId,
                new StartHearingRequest
                {
                    HearingLayout = layout.ToString(), MuteGuests = muteGuests,
                    ForceTransferParticipantIds = participantsToForceTransfer?.ToList(),
                    Hosts = hosts?.ToList(),
                    TriggeredByHostId = triggeredByHostId,
                    HostsForScreening = hostsForScreening?.ToList()
                });
        }

        public Task PauseHearingAsync(Guid conferenceId)
        {
            return _supplierApiClient.PauseHearingAsync(conferenceId);
        }
        
        public Task EndHearingAsync(Guid conferenceId)
        {
            return _supplierApiClient.EndHearingAsync(conferenceId);
        }
        
        public Task SuspendHearingAsync(Guid conferenceId)
        {
            return _supplierApiClient.TechnicalAssistanceAsync(conferenceId);
        }
        
        public Task<HealthCheckResponse> GetPlatformHealthAsync()
        {
            return _supplierApiClient.GetHealth();
        }
        
        public ISupplierApiClient GetHttpClient() => _supplierApiClient;
        
        public SupplierConfiguration GetSupplierConfiguration() => _supplierConfigOptions;
        
        public Task UpdateParticipantName(Guid conferenceId, Guid participantId, string name)
        {
            var request = new DisplayNameRequest
            {
                ParticipantId = participantId.ToString(),
                ParticipantName = name
            };
            return _supplierApiClient.UpdateParticipantDisplayNameAsync(conferenceId, request);
        }
    }
}
