using System;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Services.Contracts;
using VideoApi.Services.Kinly;
using HealthCheckResponse = VideoApi.Contract.Responses.HealthCheckResponse;

namespace VideoApi.Controllers
{
    [Produces("application/json")]
    [AllowAnonymous]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        private readonly IVideoPlatformService _videoPlatformService;
        private readonly IAudioPlatformService _audioPlatformService;

        public HealthCheckController(IQueryHandler queryHandler, IVideoPlatformService videoPlatformService,
            IAudioPlatformService audioPlatformService)
        {
            _queryHandler = queryHandler;
            _videoPlatformService = videoPlatformService;
            _audioPlatformService = audioPlatformService;
        }

        /// <summary>
        /// Check Service Health
        /// </summary>
        /// <returns>Error if fails, otherwise OK status</returns>
        [HttpGet("HealthCheck/health")]
        [HttpGet("health/liveness")]
        [OpenApiOperation("CheckServiceHealth")]
        [ProducesResponseType(typeof(HealthCheckResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(HealthCheckResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> HealthAsync()
        {
            var response = new HealthCheckResponse {AppVersion = GetApplicationVersion()};
            try
            {
                var hearingId = Guid.NewGuid();
                var query = new GetConferenceByIdQuery(hearingId);
                await _queryHandler.Handle<GetConferenceByIdQuery, Conference>(query);
                response.DatabaseHealth.Successful = true;
            }
            catch (Exception ex)
            {
                response.DatabaseHealth.Successful = false;
                response.DatabaseHealth.ErrorMessage = ex.Message;
                response.DatabaseHealth.Data = ex.Data;
            }
            
            try
            {
                await _videoPlatformService.GetTestCallScoreAsync(Guid.Empty, 0);
                response.KinlySelfTestHealth.Successful = true;
            }
            catch (Exception ex)
            {
                response.KinlySelfTestHealth.Successful = false;
                response.KinlySelfTestHealth.ErrorMessage = ex.Message;
                response.KinlySelfTestHealth.Data = ex.Data;
            }

            try
            {
                var apiHealth = await _videoPlatformService.GetPlatformHealthAsync();
                response.KinlyApiHealth.Successful = apiHealth.Health_status == PlatformHealth.HEALTHY;
            }
            catch (Exception ex)
            {
                response.KinlyApiHealth.Successful = false;
                response.KinlyApiHealth.ErrorMessage = ex.Message;
                response.KinlyApiHealth.Data = ex.Data;
            }

            try
            {
               response.WowzaHealth.Successful = false;
                var wowzaResponse = await _audioPlatformService.GetDiagnosticsAsync();
                if (wowzaResponse != null && !wowzaResponse.All(x => string.IsNullOrWhiteSpace(x.ServerVersion)))
                {
                    response.WowzaHealth.Successful = true;
                }
            }
            catch (Exception ex)
            {
                response.WowzaHealth.Successful = false;
                response.WowzaHealth.ErrorMessage = ex.Message + Environment.NewLine + ex.InnerException ;
                response.WowzaHealth.Data = ex.Data;
            }

            if (!response.DatabaseHealth.Successful || !response.KinlySelfTestHealth.Successful ||
                !response.KinlyApiHealth.Successful || !response.WowzaHealth.Successful)
            {
                return StatusCode((int)HttpStatusCode.InternalServerError, response);
            }

            return Ok(response);
        }

        private ApplicationVersion GetApplicationVersion()
        {
            var applicationVersion = new ApplicationVersion();
            applicationVersion.FileVersion = GetExecutingAssemblyAttribute<AssemblyFileVersionAttribute>(a => a.Version);
            applicationVersion.InformationVersion = GetExecutingAssemblyAttribute<AssemblyInformationalVersionAttribute>(a => a.InformationalVersion);
            return applicationVersion;
        }

        private string GetExecutingAssemblyAttribute<T>(Func<T, string> value) where T : Attribute
        {
            T attribute = (T)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(T));
            return value.Invoke(attribute);
        }
    }
}
