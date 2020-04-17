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
        /// Gets the audio application info for the conference by caseNumber and hearingId
        /// </summary>
        /// <param name="caseNumber">The case number of the conference</param>
        /// <param name="hearingId">The HearingRefId of the conference to stop the audio recording stream</param>
        /// <returns></returns>
        [HttpGet("audioapplications/{caseNumber}/{hearingId}")]
        [SwaggerOperation(OperationId = "GetAudioApplication")]
        [ProducesResponseType(typeof(AudioApplicationInfoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAudioApplicationAsync(string caseNumber, Guid hearingId)
        {
            _logger.LogDebug("GetAudioApplication");
            
            var response = await _audioPlatformService.GetAudioApplicationInfoAsync(caseNumber, hearingId);

            if (response == null) return NotFound();

            return Ok(AudioRecordingMapper.MapToAudioApplicationInfo(response));
        }

        /// <summary>
        /// Creates the audio application for the conference by caseNumber and hearingId
        /// </summary>
        /// <param name="caseNumber">The case number of the conference</param>
        /// <param name="hearingId">The HearingRefId of the conference to stop the audio recording stream</param>
        /// <returns></returns>
        [HttpPost("audioapplications/{caseNumber}/{hearingId}")]
        [SwaggerOperation(OperationId = "CreateAudioApplication")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateAudioApplicationAsync(string caseNumber, Guid hearingId)
        {
            _logger.LogDebug("CreateAudioApplication");
            
            var response = await _audioPlatformService.CreateAudioApplicationAsync(caseNumber, hearingId);

            if (!response.Success)
            {
                return StatusCode((int) response.StatusCode, response.Message);
            }

            return Ok();
        }
        
        /// <summary>
        /// Creates the audio application and associated stream for the conference by caseNumber and hearingId
        /// </summary>
        /// <param name="caseNumber">The case number of the conference</param>
        /// <param name="hearingId">The HearingRefId of the conference to stop the audio recording stream</param>
        /// <returns>The ingest url for other applications to stream to the endpoint</returns>
        [HttpPost("audioapplications/{caseNumber}/{hearingId}")]
        [SwaggerOperation(OperationId = "CreateAudioApplicationWithStream")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateAudioApplicationWithStreamAsync(string caseNumber, Guid hearingId)
        {
            _logger.LogDebug("CreateAudioApplicationWithStream");
            
            var response = await _audioPlatformService.CreateAudioApplicationWithStreamAsync(caseNumber, hearingId);

            return !response.Success ? StatusCode((int) response.StatusCode, response.Message) : Ok(response.IngestUrl);
        }
        
        /// <summary>
        /// Deletes the audio application for the conference by caseNumber and hearingId
        /// </summary>
        /// <param name="caseNumber">The case number of the conference</param>
        /// <param name="hearingId">The HearingRefId of the conference to stop the audio recording stream</param>
        /// <returns></returns>
        [HttpDelete("audioapplications/{caseNumber}/{hearingId}")]
        [SwaggerOperation(OperationId = "DeleteAudioApplication")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAudioApplicationAsync(string caseNumber, Guid hearingId)
        {
            _logger.LogDebug("DeleteAudioApplication");
            
            var response = await _audioPlatformService.DeleteAudioApplicationAsync(caseNumber, hearingId);

            if (!response.Success)
            {
                return StatusCode((int) response.StatusCode, response.Message);
            }

            return NoContent();
        }
        
        /// <summary>
        /// Gets the audio stream for the conference by caseNumber and hearingId
        /// </summary>
        /// <param name="caseNumber">The case number of the conference</param>
        /// <param name="hearingId">The HearingRefId of the conference to stop the audio recording stream</param>
        /// <returns>AudioStreamInfoResponse</returns>
        [HttpGet("audiostreams/{caseNumber}/{hearingId}")]
        [SwaggerOperation(OperationId = "GetAudioStreamInfo")]
        [ProducesResponseType(typeof(AudioStreamInfoResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAudioStreamInfoAsync(string caseNumber, Guid hearingId)
        {
            _logger.LogDebug("GetAudioStreamInfo");
            
            var response = await _audioPlatformService.GetAudioStreamInfoAsync(caseNumber, hearingId);

            if (response == null) return NotFound();

            return Ok(AudioRecordingMapper.MapToAudioStreamInfo(response));
        }
        
        /// <summary>
        /// Creates the audio stream for the conference by caseNumber and hearingId
        /// </summary>
        /// <param name="caseNumber">The case number of the conference</param>
        /// <param name="hearingId">The HearingRefId of the conference to stop the audio recording stream</param>
        /// <returns></returns>
        [HttpPost("audiostreams/{caseNumber}/{hearingId}")]
        [SwaggerOperation(OperationId = "CreateAudioStream")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateAudioStreamAsync(string caseNumber, Guid hearingId)
        {
            _logger.LogDebug("CreateAudioStream");
            
            var response = await _audioPlatformService.CreateAudioStreamAsync(caseNumber, hearingId);

            return response.Success ? Ok(response.IngestUrl) : StatusCode((int) response.StatusCode, response.Message);
        }
        
        /// <summary>
        /// Deletes the audio stream for the conference by caseNumber and hearingId
        /// </summary>
        /// <param name="caseNumber">The case number of the conference</param>
        /// <param name="hearingId">The HearingRefId of the conference to stop the audio recording stream</param>
        /// <returns></returns>
        [HttpDelete("audiostreams/{caseNumber}/{hearingId}")]
        [SwaggerOperation(OperationId = "DeleteAudioStream")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAudioStreamAsync(string caseNumber, Guid hearingId)
        {
            _logger.LogDebug("DeleteAudioStream");
            
            var response = await _audioPlatformService.DeleteAudioStreamAsync(caseNumber, hearingId);

            if (!response.Success)
            {
                return StatusCode((int) response.StatusCode, response.Message);
            }

            return NoContent();
        }
    }
}
