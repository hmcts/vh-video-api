using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VideoApi.Services.Contracts;

namespace Video.API.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("conferences")]
    [ApiController]
    public class ConferenceAudioController : ControllerBase
    {
        private readonly IAudioPlatformService _audioPlatformService;
        private readonly ILogger<ConferenceAudioController> _logger;

        public ConferenceAudioController(IAudioPlatformService audioPlatformService, ILogger<ConferenceAudioController> logger)
        {
            _audioPlatformService = audioPlatformService;
            _logger = logger;
        }
        
        
    }
}
