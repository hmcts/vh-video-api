using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Domain;
using VideoApi.Domain.Enums;
using VideoApi.Services.Clients;
using VideoApi.Services.Dtos;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Services.Contracts
{
    public interface IVideoPlatformService
    {
        /// <summary>
        /// Book a virtual courtroom
        /// </summary>
        /// <param name="conferenceId">The conference id</param>
        /// <param name="audioRecordingRequired">Is the audio recording enabled</param>
        /// <param name="ingestUrl">The ingest url used by audio recording</param>
        /// <param name="endpoints">The JVS endpoints for the conference which includes sip address, pin and role</param>
        /// <param name="telephoneId">The pin used by telephone participants to join a conference</param>
        /// <param name="roomType">The type of room, VA (Virtual Auditorium for screening) or VMR</param>
        /// <param name="audioPlaybackLanguage">The language for the conference used to determine the audio of the countdown language and waiting room message</param>
        /// <returns></returns>
        Task<MeetingRoom> BookVirtualCourtroomAsync(Guid conferenceId, bool audioRecordingRequired, string ingestUrl,
            IEnumerable<EndpointDto> endpoints, string telephoneId, ConferenceRoomType roomType,
            AudioPlaybackLanguage audioPlaybackLanguage);
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
        /// <param name="conferenceId">The conference id</param>
        /// <param name="audioRecordingRequired">Is the audio recording enabled</param>
        /// <param name="endpoints">The JVS endpoints for the conference which includes sip address, pin and role</param>
        /// <param name="roomType">The type of room, VA (Virtual Auditorium for screening) or VMR</param>
        /// <param name="audioPlaybackLanguage">The language for the conference used to determine the audio of the countdown language and waiting room message</param>
        /// <returns></returns>
        Task UpdateVirtualCourtRoomAsync(Guid conferenceId, bool audioRecordingRequired,
            IEnumerable<EndpointDto> endpoints, ConferenceRoomType roomType,
            AudioPlaybackLanguage audioPlaybackLanguage);

        Task StartHearingAsync(Guid conferenceId, string triggeredByHostId,
            IEnumerable<string> participantsToForceTransfer = null, IEnumerable<string> hosts = null,
            Layout layout = Layout.AUTOMATIC, bool muteGuests = false, IEnumerable<string> hostsForScreening = null);
        Task PauseHearingAsync(Guid conferenceId);
        Task EndHearingAsync(Guid conferenceId);
        Task SuspendHearingAsync(Guid conferenceId);
        Task<HealthCheckResponse> GetPlatformHealthAsync();
        ISupplierApiClient GetHttpClient();
        SupplierConfiguration GetSupplierConfiguration();
        Task UpdateParticipantName(Guid conferenceId, Guid participantId, string name);
    }
}
