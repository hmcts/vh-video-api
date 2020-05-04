using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Video.API.Mappings;
using VideoApi.Contract.Responses;
using VideoApi.Services.Contracts;

namespace Video.API.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("conferences")]
    [ApiController]
    public class AudioRecordingController : ControllerBase
    {
        private readonly IAudioPlatformService _audioPlatformService;
        private readonly ILogger<AudioRecordingController> _logger;

        public AudioRecordingController(IAudioPlatformService audioPlatformService, ILogger<AudioRecordingController> logger)
        {
            _audioPlatformService = audioPlatformService;
            _logger = logger;
        }

        /// <summary>
        /// Gets the audio application info for the conference by hearingId
        /// </summary>
        /// <param name="hearingId">The HearingRefId of the conference to stop the audio recording stream</param>
        /// <returns></returns>
        [HttpGet("audioapplications/{hearingId}")]
        [SwaggerOperation(OperationId = "GetAudioApplication")]
        [ProducesResponseType(typeof(AudioApplicationInfoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAudioApplicationAsync(Guid hearingId)
        {
            _logger.LogDebug("GetAudioApplication");
            
            var response = await _audioPlatformService.GetAudioApplicationInfoAsync(hearingId);

            if (response == null) return NotFound();

            return Ok(AudioRecordingMapper.MapToAudioApplicationInfo(response));
        }

        /// <summary>
        /// Creates the audio application for the conference by hearingId
        /// </summary>
        /// <param name="hearingId">The HearingRefId of the conference to stop the audio recording stream</param>
        /// <returns></returns>
        [HttpPost("audioapplications/{hearingId}")]
        [SwaggerOperation(OperationId = "CreateAudioApplication")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateAudioApplicationAsync(Guid hearingId)
        {
            _logger.LogDebug("CreateAudioApplication");
            
            var response = await _audioPlatformService.CreateAudioApplicationAsync(hearingId);

            if (!response.Success)
            {
                return StatusCode((int) response.StatusCode, response.Message);
            }

            return Ok();
        }

        /// <summary>
        /// Creates the audio application and associated stream for the conference by hearingId
        /// </summary>
        /// <param name="hearingId">The HearingRefId of the conference to stop the audio recording stream</param>
        /// <returns>The ingest url for other applications to stream to the endpoint</returns>
        [HttpPost("audioapplications/audiostream/{hearingId}")]
        [SwaggerOperation(OperationId = "CreateAudioApplicationWithStream")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateAudioApplicationWithStreamAsync(Guid hearingId)
        {
            _logger.LogDebug("CreateAudioApplicationWithStream");
            
            var response = await _audioPlatformService.CreateAudioApplicationWithStreamAsync(hearingId);

            return response.Success ? Ok(response.IngestUrl) : StatusCode((int) response.StatusCode, response.Message);
        }

        /// <summary>
        /// Deletes the audio application for the conference by hearingId
        /// </summary>
        /// <param name="hearingId">The HearingRefId of the conference to stop the audio recording stream</param>
        /// <returns></returns>
        [HttpDelete("audioapplications/{hearingId}")]
        [SwaggerOperation(OperationId = "DeleteAudioApplication")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAudioApplicationAsync(Guid hearingId)
        {
            _logger.LogDebug("DeleteAudioApplication");
            
            var response = await _audioPlatformService.DeleteAudioApplicationAsync(hearingId);

            if (!response.Success)
            {
                return StatusCode((int) response.StatusCode, response.Message);
            }

            return NoContent();
        }

        /// <summary>
        /// Gets the audio stream for the conference by hearingId
        /// </summary>
        /// <param name="hearingId">The HearingRefId of the conference to stop the audio recording stream</param>
        /// <returns>AudioStreamInfoResponse</returns>
        [HttpGet("audiostreams/{hearingId}")]
        [SwaggerOperation(OperationId = "GetAudioStreamInfo")]
        [ProducesResponseType(typeof(AudioStreamInfoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAudioStreamInfoAsync(Guid hearingId)
        {
            _logger.LogDebug("GetAudioStreamInfo");
            
            var response = await _audioPlatformService.GetAudioStreamInfoAsync(hearingId);

            if (response == null) return NotFound();

            return Ok(AudioRecordingMapper.MapToAudioStreamInfo(response));
        }

        /// <summary>
        /// Gets the audio stream for the conference by hearingId
        /// </summary>
        /// <param name="hearingId">The HearingRefId of the conference to stop the audio recording stream</param>
        /// <returns>AudioStreamInfoResponse</returns>
        [HttpGet("audiostreams/{hearingId}/monitoring")]
        [SwaggerOperation(OperationId = "GetAudioStreamMonitoringInfo")]
        [ProducesResponseType(typeof(AudioStreamMonitoringInfo), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAudioStreamMonitoringInfoAsync(Guid hearingId)
        {
            _logger.LogDebug("GetAudioStreamMonitoringInfo");
            
            var response = await _audioPlatformService.GetAudioStreamMonitoringInfoAsync(hearingId);

            if (response == null) return NotFound();

            return Ok(AudioRecordingMapper.MapToAudioStreamMonitoringInfo(response));
        }

        /// <summary>
        /// Creates the audio stream for the conference by hearingId
        /// </summary>
        /// <param name="hearingId">The HearingRefId of the conference to stop the audio recording stream</param>
        /// <returns></returns>
        [HttpPost("audiostreams/{hearingId}")]
        [SwaggerOperation(OperationId = "CreateAudioStream")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateAudioStreamAsync(Guid hearingId)
        {
            _logger.LogDebug("CreateAudioStream");
            
            var response = await _audioPlatformService.CreateAudioStreamAsync(hearingId);

            return response.Success ? Ok(response.IngestUrl) : StatusCode((int) response.StatusCode, response.Message);
        }

        /// <summary>
        /// Deletes the audio stream for the conference by hearingId
        /// </summary>
        /// <param name="hearingId">The HearingRefId of the conference to stop the audio recording stream</param>
        /// <returns></returns>
        [HttpDelete("audiostreams/{hearingId}")]
        [SwaggerOperation(OperationId = "DeleteAudioStream")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAudioStreamAsync(Guid hearingId)
        {
            _logger.LogDebug("DeleteAudioStream");
            
            var response = await _audioPlatformService.DeleteAudioStreamAsync(hearingId);

            if (!response.Success)
            {
                return StatusCode((int) response.StatusCode, response.Message);
            }

            return NoContent();
        }
    }
}
