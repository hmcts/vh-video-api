using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Exceptions;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Services.Factories;
using VideoApi.Mappings;
using VideoApi.Services.Contracts;
using VideoApi.Services.Exceptions;
using Task = System.Threading.Tasks.Task;

namespace VideoApi.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("conferences")]
    [ApiController]
    public class AudioRecordingController : ControllerBase
    {
        private readonly IAzureStorageServiceFactory _azureStorageServiceFactory;
        private readonly IAudioPlatformService _audioPlatformService;
        private readonly ILogger<AudioRecordingController> _logger;
        private readonly IQueryHandler _queryHandler;

        public AudioRecordingController(IAzureStorageServiceFactory azureStorageServiceFactory, 
            IAudioPlatformService audioPlatformService, 
            ILogger<AudioRecordingController> logger, 
            IQueryHandler queryHandler)
        {
            _azureStorageServiceFactory = azureStorageServiceFactory;
            _audioPlatformService = audioPlatformService;
            _logger = logger;
            _queryHandler = queryHandler;
        }
        
        /// <summary>
        /// Gets the audio stream for the conference by hearingId
        /// Note: Used by Video Web to determine whether or not the audio recording alert should be displayed
        /// </summary>
        /// <param name="hearingId">The HearingRefId of the conference to get the audio recording stream</param>
         /// <returns>AudioStreamInfoResponse</returns>
        [HttpGet("audiostreams/{hearingId}")]
        [OpenApiOperation("GetAudioStreamInfo")]
        [ProducesResponseType(typeof(AudioStreamInfoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAudioStreamInfoAsync(Guid hearingId)
        {
            _logger.LogDebug("GetAudioStreamInfo");
            
            try
            {
                var response = await _audioPlatformService.GetAudioStreamInfoAsync(hearingId.ToString());
                return Ok(AudioRecordingMapper.MapToAudioStreamInfo(response));
            }
            catch (AggregateException agrEx)
            {
                var exceptions = agrEx.InnerExceptions.Select(e => e.Message);
                return NotFound(exceptions);
            }
        }

        /// <summary>
        /// Get the audio recording link for a given hearing.
        /// Note: Only used by the admin web. To be decommissioned
        /// </summary>
        /// <param name="hearingReference">The hearing reference containing the hearing Id.</param>
        /// <returns> AudioRecordingResponse with the link - AudioFileLink</returns>
        [HttpGet("audio/{hearingReference}")]
        [OpenApiOperation("GetAudioRecordingLink")]
        [ProducesResponseType(typeof(AudioRecordingResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAudioRecordingLinkAsync(string hearingReference)
        {
            _logger.LogInformation("Getting audio recording link");
            try
            {
                var azureStorageService = _azureStorageServiceFactory.Create(AzureStorageServiceType.Vh);
                var allBlobNames = await azureStorageService.GetAllBlobNamesByFilePathPrefix(hearingReference);
                
                return Ok(new AudioRecordingResponse
                {
                    AudioFileLinks = allBlobNames
                        .Select(async x => await azureStorageService.CreateSharedAccessSignature(x, TimeSpan.FromDays(3)))
                        .Select(x => x.Result)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList()
                });

            }
            catch (ConferenceNotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);
                return NotFound();
            }
        }
        
        #region CVP
        /// <summary>
        /// Get the audio recording links for a given CVP recording.
        /// Note: Only used by the admin web. Need to discuss if we still need this
        /// </summary>
        /// <param name="cloudRoom"></param>
        /// <param name="date"></param>
        /// <param name="caseReference"></param>
        [HttpGet("audio/cvp/all/{cloudRoom}/{date}/{caseReference}")]
        [OpenApiOperation("GetAudioRecordingLinkAllCvp")]
        [ProducesResponseType(typeof(List<CvpAudioFileResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAudioRecordingLinkCvpAllAsync(string cloudRoom, string date, string caseReference)
        {
            _logger.LogInformation("Getting audio recording link for CVP cloud room");
            
            try
            {
                var responses = await GetCvpAudioFiles(cloudRoom, date, caseReference);

                return Ok(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return NotFound();
            }
        }

        /// <summary>
        /// Get the audio recording links for a given CVP recording.
        /// Note: Only used by the admin web. Need to discuss if we still need this
        /// </summary>
        /// <param name="cloudRoom"></param>
        /// <param name="date"></param>
        [HttpGet("audio/cvp/cloudroom/{cloudRoom}/{date}")]
        [OpenApiOperation("GetAudioRecordingLinkCvpByCloudRoom")]
        [ProducesResponseType(typeof(List<CvpAudioFileResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAudioRecordingLinkCvpByCloudRoomAsync(string cloudRoom, string date)
        {
            _logger.LogInformation("Getting audio recording link for CVP cloud room");

            try
            {
                var responses = await GetCvpAudioFiles(cloudRoom, date, null);

                return Ok(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return NotFound();
            }
        }

        /// <summary>
        /// Get the audio recording links for a given CVP recording.
        /// Note: Only used by the admin web. Need to discuss if we still need this
        /// </summary>
        /// <param name="date"></param>
        /// <param name="caseReference"></param>
        [HttpGet("audio/cvp/date/{date}/{caseReference}")]
        [OpenApiOperation("GetAudioRecordingLinkCvpByDate")]
        [ProducesResponseType(typeof(List<CvpAudioFileResponse>), (int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAudioRecordingLinkCvpByDateAsync(string date, string caseReference)
        {
            _logger.LogInformation("Getting audio recording link for CVP");

            try
            {
                var responses = await GetCvpAudioFiles(null, date, caseReference);

                return Ok(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return NotFound();
            }
        }

        private async Task<List<CvpAudioFileResponse>> GetCvpAudioFiles(string cloudRoom, string date, string caseReference)
        {
            var responses = new List<CvpAudioFileResponse>();
            var azureStorageService = _azureStorageServiceFactory.Create(AzureStorageServiceType.Cvp);
            var allBlobsAsync = azureStorageService.GetAllBlobsAsync(!string.IsNullOrWhiteSpace(cloudRoom) ? $"audiostream{cloudRoom}" : string.Empty);
            await foreach (var blob in allBlobsAsync)
            {
                var blobName = blob.Name.ToLower();
                if (!blobName.Contains(date.ToLower()) || !blobName.Contains(caseReference != null ? caseReference.ToLower() : ""))
                {
                    continue;
                }

                responses.Add(new CvpAudioFileResponse
                {
                    FileName = blob.Name.Substring(blob.Name.LastIndexOf("/") + 1),
                    SasTokenUrl = await azureStorageService.CreateSharedAccessSignature(blob.Name, TimeSpan.FromDays(3))
                });
            }

            return responses;
        }

        private async Task EnsureAudioFileExists(Conference conference, IAzureStorageService azureStorageService)
        {
            if (conference.AudioRecordingRequired)
            {
                var allBlobs = await azureStorageService.GetAllBlobNamesByFilePathPrefix(conference.HearingRefId.ToString());

                if (conference.ActualStartTime.HasValue && !allBlobs.Any())
                {
                    var msg = $"Audio recording file not found for hearing: {conference.HearingRefId}";
                    throw new AudioPlatformFileNotFoundException(msg, HttpStatusCode.NotFound);
                }
            }
        }
        
        private async Task<Conference> GetConference(Guid hearingId)
        {
            var conference =
                await _queryHandler
                   .Handle<GetConferenceByHearingRefIdQuery, Conference>(new GetConferenceByHearingRefIdQuery(hearingId));
            if (conference == null)
                throw new ConferenceNotFoundException(hearingId);
            return conference;
        }
        #endregion
        
        #region Obsolete
        /// <summary>
        /// Gets the audio application info for the conference by hearingId
        /// </summary>
        /// <param name="hearingId">The HearingRefId of the conference to retrieve the audio application info</param>
        /// <returns></returns>
        [HttpGet("audioapplications/{hearingId}")]
        [OpenApiOperation("GetAudioApplicationWithHearingId")]
        [ProducesResponseType(typeof(AudioApplicationInfoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Obsolete("We only have one application for all hearings now. Need to review old bookings.")]
        public async Task<IActionResult> GetAudioApplicationAsync(Guid hearingId)
        {
            _logger.LogDebug("GetAudioApplicationWithHearingId");

            var response = await _audioPlatformService.GetAudioApplicationInfoAsync(hearingId);

            if (response == null) return NotFound();

            return Ok(AudioRecordingMapper.MapToAudioApplicationInfo(response));
        }
                
        /// <summary>
        /// Gets the audio application info
        /// </summary>
        /// <returns></returns>
        [HttpGet("audioapplications")]
        [OpenApiOperation("GetAudioApplication")]
        [ProducesResponseType(typeof(AudioApplicationInfoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Obsolete("We only have one application for all hearings now. Need to review old bookings.")]
        public async Task<IActionResult> GetAudioApplicationAsync()
        {
            _logger.LogDebug("GetAudioApplication");
            return Ok(AudioRecordingMapper.MapToAudioApplicationInfo(await _audioPlatformService.GetAudioApplicationInfoAsync()));
        }
        
        /// <summary>
        /// Deletes the audio application for the conference by hearingId
        /// </summary>
        /// <param name="hearingId">The HearingRefId of the conference to stop the audio recording application</param>
        /// <returns></returns>
        [HttpDelete("audioapplications/{hearingId}")]
        [OpenApiOperation("DeleteAudioApplication")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Obsolete("We only have one application for all hearings now. Need to review old bookings.")]
        public async Task<IActionResult> DeleteAudioApplicationAsync(Guid hearingId)
        {
            _logger.LogDebug("DeleteAudioApplication");

            try
            {
                var conference = await GetConference(hearingId);

                //Hearings recorded to single app instance to be skipped (only one shared application, so cant be deleted)
                if (conference.IngestUrl.Contains(_audioPlatformService.ApplicationName))
                    return NoContent();
                
                await EnsureAudioFileExists(conference, _azureStorageServiceFactory.Create(AzureStorageServiceType.Vh));
            }
            catch (Exception ex) when (ex is AudioPlatformFileNotFoundException || ex is ConferenceNotFoundException)
            {
                _logger.LogError(ex, ex.Message);
                return NotFound();
            }

            var response = await _audioPlatformService.DeleteAudioApplicationAsync(hearingId);

            if (!response.Success)
            {
                return StatusCode((int)response.StatusCode, response.Message);
            }
            return NoContent();
        }
        /// <summary>
        /// Gets the audio stream for monitoring the conference by hearingId
        /// </summary>
        /// <param name="hearingId">The HearingRefId of the conference to monitor the audio recording stream</param>
        /// <returns>AudioStreamInfoResponse</returns>
        [HttpGet("audiostreams/{hearingId}/monitoring")]
        [OpenApiOperation("GetAudioStreamMonitoringInfo")]
        [ProducesResponseType(typeof(AudioStreamMonitoringInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Obsolete("We only have one application for all hearings now. Need to review old bookings.")]
        public async Task<IActionResult> GetAudioStreamMonitoringInfoAsync(Guid hearingId)
        {
            _logger.LogDebug("GetAudioStreamMonitoringInfo");

            var response = await _audioPlatformService.GetAudioStreamMonitoringInfoAsync(hearingId);

            if (response == null) return NotFound();

            return Ok(AudioRecordingMapper.MapToAudioStreamMonitoringInfo(response));
        }
        #endregion
    }
}
