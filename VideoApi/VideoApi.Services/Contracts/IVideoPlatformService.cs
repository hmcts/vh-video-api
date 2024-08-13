using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Dtos;
using VideoApi.Services.Clients;
using Endpoint = VideoApi.Domain.Endpoint;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services.Contracts
{
    public interface IVideoPlatformService
    {
        Task<MeetingRoom> BookVirtualCourtroomAsync(Guid conferenceId, bool audioRecordingRequired, string ingestUrl, IEnumerable<EndpointDto> endpoints);
        Task<MeetingRoom> GetVirtualCourtRoomAsync(Guid conferenceId);
        Task<TestCallResult> GetTestCallScoreAsync(Guid participantId, int retryAttempts = 2);
        Task TransferParticipantAsync(Guid conferenceId, string participantId, string fromRoom, string toRoom);
        
        /// <summary>
        /// Delete virtual court room
        /// </summary>
        /// <param name="conferenceId">Conference Id</param>
        /// <returns></returns>
        Task DeleteVirtualCourtRoomAsync(Guid conferenceId);

        /// <summary>
        /// Update virtual court room
        /// </summary>
        /// <param name="conferenceId"></param>
        /// <param name="audioRecordingRequired"></param>
        /// <returns></returns>
        Task UpdateVirtualCourtRoomAsync(Guid conferenceId, bool audioRecordingRequired, IEnumerable<EndpointDto> endpoints);

        Task StartHearingAsync(Guid conferenceId, string triggeredByHostId, IEnumerable<string> participantsToForceTransfer = null, Layout layout = Layout.AUTOMATIC, bool muteGuests = true);
        
        Task PauseHearingAsync(Guid conferenceId);
        
        Task EndHearingAsync(Guid conferenceId);

        Task SuspendHearingAsync(Guid conferenceId);

        Task<HealthCheckResponse> GetPlatformHealthAsync();
        
        ISupplierApiClient GetHttpClient();
        SupplierConfiguration GetSupplierConfiguration();
    }
}
