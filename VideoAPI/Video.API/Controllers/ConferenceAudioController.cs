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
        private readonly IConferenceStreamingService _conferenceStreamingService;
        private readonly ILogger<ConferenceAudioController> _logger;

        public ConferenceAudioController(IConferenceStreamingService conferenceStreamingService, ILogger<ConferenceAudioController> logger)
        {
            _conferenceStreamingService = conferenceStreamingService;
            _logger = logger;
        }
        
        
    }
}
