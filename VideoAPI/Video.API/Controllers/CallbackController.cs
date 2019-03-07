using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VideoApi.Contract.Requests;

namespace Video.API.Controllers
{
    [Consumes("application/json")]
    [Produces("application/json")]
    [Route("callback")]
    [ApiController]
    public class CallbackController : ControllerBase
    {
        /// <summary>
        /// Accepts video conference events and publishes to internal clients/services
        /// </summary>
        /// <param name="request">Details of the event</param>
        /// <returns>OK if event is handled as expected</returns>
        [HttpPost("conference")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> PostEvents(ConferenceEventRequest request)
        {
            throw new NotImplementedException();
        }
    }
}