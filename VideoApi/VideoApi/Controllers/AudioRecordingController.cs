using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Exceptions;
using VideoApi.Services.Contracts;
using VideoApi.Services.Factories;

namespace VideoApi.Controllers;

[Consumes("application/json")]
[Produces("application/json")]
[Route("conferences")]
[ApiController]
public class AudioRecordingController(
    IAzureStorageServiceFactory azureStorageServiceFactory,
    ILogger<AudioRecordingController> logger)
    : ControllerBase
{
    /// <summary>
    /// Get the audio recording link for a given hearing.
    /// Note: Only used by the admin webL lower environments only
    /// </summary>
    /// <param name="hearingReference">The hearing reference containing the hearing Id.</param>
    /// <returns> AudioRecordingResponse with the link - AudioFileLink</returns>
    [HttpGet("audio/{hearingReference}")]
    [OpenApiOperation("GetAudioRecordingLink")]
    [ProducesResponseType(typeof(AudioRecordingResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetAudioRecordingLinkAsync(string hearingReference)
    {
        logger.LogInformation("Getting audio recording link");
        try
        {
            var azureStorageService = azureStorageServiceFactory.Create(AzureStorageServiceType.Vh);
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
            logger.LogError(ex, "Not found: {Message}", ex.Message);
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
        logger.LogInformation("Getting audio recording link for CVP cloud room");
        
        try
        {
            var responses = await GetCvpAudioFiles(cloudRoom, date, caseReference);
            
            return Ok(responses);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error: {Message}", ex.Message);
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
        logger.LogInformation("Getting audio recording link for CVP cloud room");
        
        try
        {
            var responses = await GetCvpAudioFiles(cloudRoom, date, null);
            
            return Ok(responses);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error: {Message}", ex.Message);
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
        logger.LogInformation("Getting audio recording link for CVP");
        
        try
        {
            var responses = await GetCvpAudioFiles(null, date, caseReference);
            
            return Ok(responses);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error: {Message}", ex.Message);
            return NotFound();
        }
    }
    
    private async Task<List<CvpAudioFileResponse>> GetCvpAudioFiles(string cloudRoom, string date, string caseReference)
    {
        var responses = new List<CvpAudioFileResponse>();
        var azureStorageService = azureStorageServiceFactory.Create(AzureStorageServiceType.Cvp);
        var allBlobsAsync = azureStorageService.GetAllBlobsAsync(!string.IsNullOrWhiteSpace(cloudRoom) ? $"audiostream{cloudRoom}" : string.Empty);
        await foreach (var blob in allBlobsAsync)
        {
            var blobName = blob.Name.ToLower();
            if (!blobName.Contains(date, StringComparison.CurrentCultureIgnoreCase) || !blobName.Contains(caseReference != null ? caseReference.ToLower() : ""))
            {
                continue;
            }
            
            responses.Add(new CvpAudioFileResponse
            {
                FileName = blob.Name.Substring(blob.Name.LastIndexOf('/') + 1),
                SasTokenUrl = await azureStorageService.CreateSharedAccessSignature(blob.Name, TimeSpan.FromDays(3))
            });
        }
        
        return responses;
    }
    
    #endregion
}
