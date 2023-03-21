using System;
using System.Threading.Tasks;
using VideoApi.Contract.Responses;
using VideoApi.Services.Responses;

namespace VideoApi.Services.Contracts
{
    public interface IAudioPlatformService
    {
        /// <summary>
        /// Used to delete the 'old-style' wowza application per hearing model. Will eventually not be needed
        /// once no mode hearings booked in that style exist
        /// </summary>
        Task<AudioPlatformServiceResponse> DeleteAudioApplicationAsync(Guid hearingId);         
        
        /// <summary>
        /// Returns the status of the in-progress hearing recorder. Only requires that one node return a successful response
        /// Will throw aggregate exception if the streaming recorder can not be found on both Wowza Vms
        /// </summary>
        Task<WowzaGetStreamRecorderResponse> GetAudioStreamInfoAsync(string application, string recorder);
        
        /// <summary>
        /// Builds the URI that Kinly will stream the audio to on the wowza node. 
        /// </summary>
        public string GetAudioIngestUrl(string hearingId);
        
        /// <summary>
        /// Wowza recorder application name
        /// </summary>
        public string ApplicationName { get; }
        
        /// <summary>
        /// Use to check the status of the vms for the video-api health check. Checks status of load balancer, as
        /// only requires one health VM to return success
        /// </summary>
        Task<bool> GetDiagnosticsAsync();
        
        [Obsolete]
        Task<WowzaGetApplicationResponse> GetAudioApplicationInfoAsync(Guid? hearingId = null);   
        
        [Obsolete]   
        Task<WowzaMonitorStreamResponse> GetAudioStreamMonitoringInfoAsync(Guid hearingId); 
    }
}
