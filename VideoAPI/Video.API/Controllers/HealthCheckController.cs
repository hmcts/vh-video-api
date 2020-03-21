using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using VideoApi.Contract.Responses;
using VideoApi.DAL.Queries;
using VideoApi.DAL.Queries.Core;
using VideoApi.Domain;
using VideoApi.Services;
using System.Reflection;

namespace Video.API.Controllers
{
    [Produces("application/json")]
    [Route("HealthCheck")]
    [AllowAnonymous]
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
        private readonly IQueryHandler _queryHandler;
        private readonly IVideoPlatformService _videoPlatformService;

        public HealthCheckController(IQueryHandler queryHandler, IVideoPlatformService videoPlatformService)
        {
            _queryHandler = queryHandler;
            _videoPlatformService = videoPlatformService;
        }

        /// <summary>
        /// Check Service Health
        /// </summary>
        /// <returns>Error if fails, otherwise OK status</returns>
        [HttpGet("health")]
        [SwaggerOperation(OperationId = "CheckServiceHealth")]
        [ProducesResponseType(typeof(HealthCheckResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(HealthCheckResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> HealthAsync()
        {
            var response = new HealthCheckResponse();
            response.AppVersion = GetApplicationVersion();
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
                await _videoPlatformService.GetTestCallScoreAsync(Guid.Empty);
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
                await _videoPlatformService.GetVirtualCourtRoomAsync(Guid.Empty);
                response.KinlyApiHealth.Successful = true;
            }
            catch (Exception ex)
            {
                response.KinlyApiHealth.Successful = false;
                response.KinlyApiHealth.ErrorMessage = ex.Message;
                response.KinlyApiHealth.Data = ex.Data;
            }

            if (!response.DatabaseHealth.Successful || !response.KinlySelfTestHealth.Successful ||
                !response.KinlyApiHealth.Successful)
            {
                return StatusCode((int) HttpStatusCode.InternalServerError, response);
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
