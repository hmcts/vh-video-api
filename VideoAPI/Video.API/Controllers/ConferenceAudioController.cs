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
        private readonly IAudioStreamService _audioStreamService;
        private readonly ILogger<ConferenceAudioController> _logger;

        public ConferenceAudioController(IAudioStreamService audioStreamService, ILogger<ConferenceAudioController> logger)
        {
            _audioStreamService = audioStreamService;
            _logger = logger;
        }
        
        
    }
}
