using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Clients;
using VideoApi.Services.Contracts;
using VideoApi.Services.Dtos;
using VideoApi.Services.Exceptions;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services
{
    [ExcludeFromCodeCoverage]
    public class SupplierPlatformServiceStub : IVideoPlatformService
    {
        private readonly List<Guid> _bookedGuids = new();
        private readonly SupplierConfiguration _supplierConfiguration;
        
        public SupplierPlatformServiceStub(SupplierConfiguration supplierConfiguration)
        {
            _supplierConfiguration = supplierConfiguration;
        }
        
        public Task<MeetingRoom> BookVirtualCourtroomAsync(Guid conferenceId,
            bool audioRecordingRequired,
            string ingestUrl,
            IEnumerable<EndpointDto> endpoints, string telephoneId, ConferenceRoomType roomType,
            AudioPlaybackLanguage audioPlaybackLanguage)
        {
            if (_bookedGuids.Contains(conferenceId))
                throw new DoubleBookingException(conferenceId);

            var meetingRoom = Create();
            _bookedGuids.Add(conferenceId);
            return Task.FromResult(meetingRoom);
        }
        
        public Task<MeetingRoom> GetVirtualCourtRoomAsync(Guid conferenceId)
        {
            return Task.FromResult(Create());
        }
        
        public Task<TestCallResult> GetTestCallScoreAsync(Guid participantId,int retryAttempts = 2)
        {
            return Task.FromResult<TestCallResult>(null);
        }
        
        public Task TransferParticipantAsync(Guid conferenceId, string participantId, string fromRoom, string toRoom)
        {
            return Task.CompletedTask;
        }
        
        public Task DeleteVirtualCourtRoomAsync(Guid conferenceId)
        {
            return Task.CompletedTask;
        }
        
        public Task UpdateVirtualCourtRoomAsync(Guid conferenceId, bool audioRecordingRequired, 
            IEnumerable<EndpointDto> endpoints, ConferenceRoomType roomType,
            AudioPlaybackLanguage audioPlaybackLanguage)
        {
            return Task.CompletedTask;
        }
        
        public Task StartHearingAsync(Guid conferenceId, string triggeredByHostId, IEnumerable<string> participantsToForceTransfer = null,
            IEnumerable<string> hosts = null, Layout layout = Layout.AUTOMATIC, bool muteGuests = false, IEnumerable<string> hostsForScreening = null)
        {
            return Task.CompletedTask;
        }
        
        public Task PauseHearingAsync(Guid conferenceId)
        {
            return Task.CompletedTask;
        }
        
        public Task EndHearingAsync(Guid conferenceId)
        {
            return Task.CompletedTask;
        }
        
        public Task SuspendHearingAsync(Guid conferenceId)
        {
            return Task.CompletedTask;
        }
        
        public Task<HealthCheckResponse> GetPlatformHealthAsync()
        {
            return Task.FromResult(new HealthCheckResponse
            {
                Health_status = PlatformHealth.HEALTHY
            });
        }
        
        public ISupplierApiClient GetHttpClient()
        {
            throw new NotImplementedException();
        }
        
        public SupplierConfiguration GetSupplierConfiguration() => _supplierConfiguration;
        
        public Task UpdateParticipantName(Guid conferenceId, Guid participantId, string name)
        {
            return Task.CompletedTask;
        }
        
        private static MeetingRoom Create()
        {
            const string URL_CONFERENCE = "https://ext-node02.com/webapp/#/?conference=user@hmcts.net";
            var adminUri = URL_CONFERENCE;
            var judgeUri = URL_CONFERENCE;
            var participantUri = URL_CONFERENCE;
            var pexipNode = "join.node.com";
            var telephoneConferenceId = "12345678";
            return new MeetingRoom(adminUri, judgeUri, participantUri, pexipNode, telephoneConferenceId);
        }
    }
}
