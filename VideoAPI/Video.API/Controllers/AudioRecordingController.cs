using System;
using System.Collections.Generic;
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
        /// Deletes the audio application for the conference by caseNumber and hearingId
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
            
            var response = await _audioPlatformService.CreateAudioStreamAsync(caseNumber, hearingId);

            if (!response.Success)
            {
                return StatusCode((int) response.StatusCode);
            }

            return Ok(response.IngestUrl);
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
            
            var response = await _audioPlatformService.DeleteAudioStreamAsync(caseNumber, hearingId);

            return !response.Success ? StatusCode((int) response.StatusCode) : NoContent();
        }
        
        /// <summary>
        /// Gets the audio stream for the conference by caseNumber and hearingId
        /// </summary>
        /// <param name="caseNumber">The case number of the conference</param>
        /// <param name="hearingId">The HearingRefId of the conference to stop the audio recording stream</param>
        /// <returns></returns>
        [HttpGet("audiostreams/{caseNumber}/{hearingId}")]
        [SwaggerOperation(OperationId = "GetAudioStreamInfo")]
        [ProducesResponseType(typeof(List<InstantMessageResponse>), StatusCodes.Status200OK)]
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
        /// Deletes the audio stream for the conference by caseNumber and hearingId
        /// </summary>
        /// <param name="caseNumber">The case number of the conference</param>
        /// <param name="hearingId">The HearingRefId of the conference to stop the audio recording stream</param>
        /// <returns></returns>
        [HttpDelete("audiostreams/{caseNumber}/{hearingId}")]
        [SwaggerOperation(OperationId = "RemoveAudioStream")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveAudioStreamAsync(string caseNumber, Guid hearingId)
        {
            _logger.LogDebug("RemoveAudioStream");
            
            var response = await _audioPlatformService.StopAudioStreamAsync(caseNumber, hearingId);

            return !response.Success ? StatusCode((int) response.StatusCode) : NoContent();
        }
    }
}
