using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using VideoApi.Common.Security.Supplier.Base;
using VideoApi.Contract.Enums;
using VideoApi.Contract.Responses;
using VideoApi.Mappings;
using VideoApi.Services;

namespace VideoApi.Controllers
{
    [Produces("application/json")]
    [Route("selftest")]
    [ApiController]
    public class SelfTestController : Controller
    {
        private readonly ISupplierPlatformServiceFactory _supplierPlatformServiceFactory;
        private readonly ILogger<SelfTestController> _logger;

        public SelfTestController(ISupplierPlatformServiceFactory supplierPlatformServiceFactory,
            ILogger<SelfTestController> logger)
        {
            _supplierPlatformServiceFactory = supplierPlatformServiceFactory;
            _logger = logger;
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

            var supplierPlatformService = _supplierPlatformServiceFactory.Create(Supplier.Kinly);
            var supplierConfiguration = supplierPlatformService.GetSupplierConfiguration();
            
            if (supplierConfiguration == null)
            {
                _logger.LogWarning($"Unable to retrieve the pexip configuration!");
                
                return NotFound();
            }
            var response = PexipConfigurationMapper.MapPexipConfigToResponse(supplierConfiguration);
            return Ok(response);
        }
    }
}
