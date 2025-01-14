using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;
using VideoApi.Contract.Responses;
using VideoApi.Mappings;
using VideoApi.Services;
using Supplier = VideoApi.Domain.Enums.Supplier;

namespace VideoApi.Controllers
{
    [Produces("application/json")]
    [Route("selftest")]
    [ApiController]
    public class SelfTestController(
        ISupplierPlatformServiceFactory supplierPlatformServiceFactory,
        ILogger<SelfTestController> logger)
        : ControllerBase
    {
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
            logger.LogDebug($"GetPexipServicesConfiguration");
            
            var supplierPlatformService = supplierPlatformServiceFactory.Create(Supplier.Vodafone);
            var supplierConfiguration = supplierPlatformService.GetSupplierConfiguration();

            if (supplierConfiguration == null)
            {
                logger.LogWarning($"Unable to retrieve the pexip configuration!");

                return NotFound();
            }
            var response = PexipConfigurationMapper.MapPexipConfigToResponse(supplierConfiguration);
            return Ok(response);
        }
    }
}
