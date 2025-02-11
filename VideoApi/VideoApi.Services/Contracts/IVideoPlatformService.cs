using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Clients;
using VideoApi.Services.Clients.Models;
using VideoApi.Services.Dtos;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services.Contracts
{
    public interface IVideoPlatformService
    {
        Task<MeetingRoom> BookVirtualCourtroomAsync(Guid conferenceId, bool audioRecordingRequired, string ingestUrl,
            IEnumerable<EndpointDto> endpoints, string telephoneId, ConferenceRoomType roomType,
            AudioPlaybackLanguage audioPlaybackLanguage);
        
        Task<MeetingRoom> GetVirtualCourtRoomAsync(Guid conferenceId);
        Task<TestCallResult> GetTestCallScoreAsync(Guid participantId, int retryAttempts = 2);
        Task TransferParticipantAsync(Guid conferenceId, string participantId, string fromRoom, string toRoom, ConferenceRole? role = null);
        Task DeleteVirtualCourtRoomAsync(Guid conferenceId);
        Task UpdateVirtualCourtRoomAsync(Guid conferenceId, bool audioRecordingRequired,
            IEnumerable<EndpointDto> endpoints, ConferenceRoomType roomType,
            AudioPlaybackLanguage audioPlaybackLanguage);
        
        Task StartHearingAsync(Guid conferenceId, string triggeredByHostId,
            IEnumerable<string> participantsToForceTransfer = null, IEnumerable<string> hosts = null,
            Layout layout = Layout.Automatic, bool muteGuests = false, IEnumerable<string> hostsForScreening = null);
        
        Task PauseHearingAsync(Guid conferenceId);
        Task EndHearingAsync(Guid conferenceId);
        Task SuspendHearingAsync(Guid conferenceId);
        Task<HealthCheckResponse> GetPlatformHealthAsync();
        ISupplierApiClient GetHttpClient();
        SupplierConfiguration GetSupplierConfiguration();
        Task UpdateParticipantName(Guid conferenceId, Guid participantId, string name);
    }
}
