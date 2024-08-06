using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Domain;
using VideoApi.Services.Exceptions;
using VideoApi.Services.Clients;
using Task = System.Threading.Tasks.Task;
using VideoApi.Services.Contracts;
using VideoApi.Services.Dtos;
using VideoApi.Services.Mappers;
using Supplier = VideoApi.Domain.Enums.Supplier;

namespace VideoApi.Services
{
    public class SupplierPlatformService : IVideoPlatformService
    {
        private readonly ILogger<SupplierPlatformService> _logger;
        private readonly ISupplierApiClient _supplierApiClient;
        private readonly SupplierConfiguration _supplierConfigOptions;
        private readonly ISupplierSelfTestHttpClient _supplierSelfTestHttpClient;
        private readonly IPollyRetryService _pollyRetryService;
        private readonly Supplier _supplier;

        public SupplierPlatformService(
            ILogger<SupplierPlatformService> logger,
            ISupplierSelfTestHttpClient supplierSelfTestHttpClient,
            IPollyRetryService pollyRetryService,
            ISupplierApiClient supplierApiClient,
            SupplierConfiguration supplierConfiguration,
            Supplier supplier)
        {
            _logger = logger;
            _supplierSelfTestHttpClient = supplierSelfTestHttpClient;
            _pollyRetryService = pollyRetryService;
            _supplierApiClient = supplierApiClient;
            _supplierConfigOptions = supplierConfiguration;
            _supplier = supplier;
        }


        public async Task<MeetingRoom> BookVirtualCourtroomAsync(Guid conferenceId,
            bool audioRecordingRequired,
            string ingestUrl,
            IEnumerable<EndpointDto> endpoints)
        {
            _logger.LogInformation(
                "Booking a conference for {ConferenceId} with callback {CallbackUri} at {KinlyApiUrl}", conferenceId,
                _supplierConfigOptions.CallbackUri, _supplierConfigOptions.ApiUrl);

            try
            {
                var response = await _supplierApiClient.CreateHearingAsync(new CreateHearingParams
                {
                    Virtual_courtroom_id = conferenceId.ToString(),
                    Callback_uri = _supplierConfigOptions.CallbackUri,
                    Recording_enabled = audioRecordingRequired,
                    Recording_url = ingestUrl,
                    Streaming_enabled = false,
                    Streaming_url = null,
                    Jvs_endpoint = endpoints.Select(EndpointMapper.MapToEndpoint).ToList()
                });
                // vodafone telephone conference id not yet implemented so a made up value is in place to avoid null reference exception
                // TODO: remove the temp value at the next milestone
                return _supplier == Supplier.Vodafone 
                    ? new MeetingRoom(response.Uris.Admin ?? response.Uris.Participant, response.Uris.Participant, response.Uris.Participant, response.Uris.Pexip_node, response.Telephone_conference_id ?? "99173907")
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
                        "Failed to retrieve test score for participant {ParticipantId} at {KinlySelfTestApiUrl}. Retrying attempt {retryAttempt}",
                        participantId, _supplierConfigOptions.ApiUrl, retryAttempt),
                callResult => callResult == null,
                () => _supplierSelfTestHttpClient.GetTestCallScoreAsync(participantId)
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
                Part_id = participantId
            };

            return _supplierApiClient.TransferParticipantAsync(conferenceId.ToString(), request);
        }

        public Task DeleteVirtualCourtRoomAsync(Guid conferenceId)
        {
            return _supplierApiClient.DeleteHearingAsync(conferenceId.ToString());
        }

        public Task UpdateVirtualCourtRoomAsync(Guid conferenceId, bool audioRecordingRequired,
            IEnumerable<EndpointDto> endpoints)
        {
            return _supplierApiClient.UpdateHearingAsync(conferenceId.ToString(),
                new UpdateHearingParams
                {
                    Recording_enabled = audioRecordingRequired,
                    Jvs_endpoint = endpoints.Select(EndpointMapper.MapToEndpoint).ToList()
                });
        }

        public Task StartHearingAsync(Guid conferenceId, string triggeredByHostId, IEnumerable<string> participantsToForceTransfer = null, Layout layout = Layout.AUTOMATIC, bool muteGuests = false)
        {
            if(_supplier != Supplier.Vodafone)
                triggeredByHostId = null;
            return _supplierApiClient.StartAsync(conferenceId.ToString(),
                new StartHearingRequest { Hearing_layout = layout, Mute_guests = muteGuests, Force_transfer_participant_ids = participantsToForceTransfer?.ToList(), Triggered_by_host_id = triggeredByHostId});
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
    }
}
