﻿using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSwag.Annotations;
using VideoApi.Common.Security.Kinly;
using VideoApi.Contract.Responses;
using VideoApi.Mappings;

namespace VideoApi.Controllers
{
    [Produces("application/json")]
    [Route("selftest")]
    [ApiController]
    public class SelfTestController : Controller
    {
        private readonly ILogger<SelfTestController> _logger;
        private readonly KinlyConfiguration _kinlyConfiguration;

        public SelfTestController(IOptions<KinlyConfiguration> kinlyConfiguration,
            ILogger<SelfTestController> logger)
        {
            _logger = logger;
            _kinlyConfiguration = kinlyConfiguration.Value;
        }

        /// <summary>
        /// Get the pexip service configuration.
        /// </summary>
        /// <returns>Returns the pexip node</returns>
        [HttpGet]
        [OpenApiOperation("GetPexipServicesConfiguration")]
        [ProducesResponseType(typeof(PexipConfigResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public IActionResult GetPexipServicesConfiguration()
        {
            _logger.LogDebug($"GetPexipServicesConfiguration");

            if (_kinlyConfiguration == null)
            {
                _logger.LogWarning($"Unable to retrieve the pexip configuration!");
                
                return NotFound();
            }
            var response = PexipConfigurationMapper.MapPexipConfigToResponse(_kinlyConfiguration);
            return Ok(response);
        }
    }
}
