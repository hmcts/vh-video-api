using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Dtos;
using VideoApi.Services.Kinly;
using Endpoint = VideoApi.Domain.Endpoint;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services.Contracts
{
    public interface IVideoPlatformService
    {
        Task<MeetingRoom> BookVirtualCourtroomAsync(Guid conferenceId, bool audioRecordingRequired, string ingestUrl, IEnumerable<EndpointDto> endpoints);
        Task<MeetingRoom> GetVirtualCourtRoomAsync(Guid conferenceId);
        Task<TestCallResult> GetTestCallScoreAsync(Guid participantId, int retryAttempts = 2);
        Task TransferParticipantAsync(Guid conferenceId, Guid participantId, string fromRoom, string toRoom);
        
        /// <summary>
        /// Returns participants in a given room to waiting room
        /// </summary>
        /// <param name="conference"></param>
        /// <param name="consultationRoom"></param>
        /// <returns></returns>
        Task StopPrivateConsultationAsync(Conference conference, string consultationRoom);

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
        
        Task StartHearingAsync(Guid conferenceId, Layout layout = Layout.AUTOMATIC);
        
        Task PauseHearingAsync(Guid conferenceId);
        
        Task EndHearingAsync(Guid conferenceId);

        Task SuspendHearingAsync(Guid conferenceId);

        Task<HealthCheckResponse> GetPlatformHealthAsync();
    }
}
