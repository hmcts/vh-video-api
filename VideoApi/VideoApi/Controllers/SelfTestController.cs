﻿using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSwag.Annotations;
using Video.API.Mappings;
using VideoApi.Common.Configuration;
using VideoApi.Contract.Responses;

namespace Video.API.Controllers
{
    [Produces("application/json")]
    [Route("selftest")]
    [ApiController]
    public class SelfTestController : Controller
    {
        private readonly ILogger<SelfTestController> _logger;
        private readonly ServicesConfiguration _servicesConfiguration;

        public SelfTestController(IOptions<ServicesConfiguration> servicesConfiguration,
            ILogger<SelfTestController> logger)
        {
            _logger = logger;
            _servicesConfiguration = servicesConfiguration.Value;
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

            if (_servicesConfiguration == null)
            {
                _logger.LogWarning($"Unable to retrieve the pexip configuration!");
                
                return NotFound();
            }
            var response = PexipConfigurationMapper.MapPexipConfigToResponse(_servicesConfiguration);
            return Ok(response);
        }
    }
}
